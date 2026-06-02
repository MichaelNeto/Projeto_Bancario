using System.Text.Json;
using BancoApi.Data;
using BancoApi.Domain;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Services;

public class PixProcessingConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly KafkaProducerService _producer;
    private readonly IConsumer<Ignore, string> _consumer;

    public PixProcessingConsumer(IServiceScopeFactory scopeFactory, KafkaProducerService producer, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _producer = producer;

        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "pix-processamento-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            try
            {
                _consumer.Subscribe("pix-solicitado");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = _consumer.Consume(stoppingToken);
                        if (result == null || string.IsNullOrWhiteSpace(result.Message?.Value))
                        {
                            continue;
                        }

                        using var document = JsonDocument.Parse(result.Message.Value);
                        var root = document.RootElement;
                        var idTransacao = root.GetProperty("idTransacao").GetString();
                        var valor = root.GetProperty("financeiro").GetProperty("valor").GetDecimal();
                        var chaveDestino = root.GetProperty("destino").GetProperty("chavePix").GetString() ?? string.Empty;
                        var origemConta = root.GetProperty("origem").GetProperty("conta").GetString() ?? string.Empty;

                        if (!Guid.TryParse(idTransacao, out var transacaoId))
                        {
                            continue;
                        }

                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<BancoContext>();
                        var destino = await db.Clientes.FirstOrDefaultAsync(c => c.ChavePix == chaveDestino, stoppingToken);
                        var transacao = await db.Transacoes.Include(t => t.ContaOrigem).FirstOrDefaultAsync(t => t.Id == transacaoId, stoppingToken);

                        if (transacao is null)
                        {
                            continue;
                        }

                        if (destino is null || destino.StatusCliente != StatusCliente.ATIVO)
                        {
                            await using var dbTransactionFail = await db.Database.BeginTransactionAsync(stoppingToken);

                            if (transacao.ContaOrigem is not null)
                            {
                                transacao.ContaOrigem.Saldo += valor;
                            }

                            transacao.Status = StatusTransacao.Falhou;
                            transacao.Observacao = "PIX falhou: destino inválido ou inativo. Saldo estornado.";

                            await db.SaveChangesAsync(stoppingToken);
                            await dbTransactionFail.CommitAsync(stoppingToken);

                            var estorno = new
                            {
                                evento = "PIX_ESTORNADO",
                                idTransacao = transacao.Id.ToString(),
                                dataHora = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                statusFinal = "FALHA",
                                origem = new
                                {
                                    documento = transacao.ContaOrigem?.DocumentoPrincipal ?? string.Empty,
                                    agencia = transacao.ContaOrigem?.NumeroAgencia ?? string.Empty,
                                    conta = transacao.ContaOrigem?.NumeroConta ?? string.Empty
                                },
                                destino = new
                                {
                                    chavePix = chaveDestino
                                },
                                valor = valor
                            };

                            await _producer.ProduceAsync("pix-estornado", estorno);
                            continue;
                        }

                        await using var dbTransaction = await db.Database.BeginTransactionAsync(stoppingToken);
                        destino.Saldo += valor;
                        transacao.Status = StatusTransacao.Concluido;
                        transacao.Tipo = TipoTransacao.PIX_ENVIADO;

                        var transacaoRecebimento = new Transacao
                        {
                            ContaOrigemId = transacao.ContaOrigemId,
                            ContaDestinoId = destino.Id,
                            Tipo = TipoTransacao.PIX_RECEBIDO,
                            Status = StatusTransacao.Concluido,
                            Valor = valor,
                            Observacao = "PIX recebido"
                        };

                        db.Transacoes.Add(transacaoRecebimento);
                        await db.SaveChangesAsync(stoppingToken);
                        await dbTransaction.CommitAsync(stoppingToken);

                        var payload = new
                        {
                            evento = "PIX_CONCLUIDO",
                            idTransacao = transacao.Id.ToString(),
                            dataHoraLiquidacao = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            statusFinal = "SUCESSO",
                            pagador = new
                            {
                                conta = transacao.ContaOrigem?.NumeroConta ?? string.Empty,
                                nome = transacao.ContaOrigem?.Nome ?? string.Empty
                            },
                            favorecido = new
                            {
                                conta = destino.NumeroConta,
                                nome = destino.Nome,
                                chavePix = destino.ChavePix
                            },
                            valor = valor
                        };

                        await _producer.ProduceAsync("pix-concluido", payload);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"PixProcessingConsumer error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PixProcessingConsumer fatal error: {ex.Message}");
            }
        }, stoppingToken);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
