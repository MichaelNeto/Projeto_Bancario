using System.Text.Json;
using BancoApi.Data;
using BancoApi.Domain;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Services;

public class CreditApprovalConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly KafkaProducerService _producer;
    private readonly IConsumer<Ignore, string> _consumer;

    public CreditApprovalConsumer(IServiceScopeFactory scopeFactory, KafkaProducerService producer, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _producer = producer;

        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "credito-concessao-group",
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
                _consumer.Subscribe("cliente-cadastrado");

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
                        var clienteElement = root.GetProperty("cliente");
                        var documento = clienteElement.GetProperty("documento").GetString() ?? string.Empty;
                        var renda = clienteElement.GetProperty("rendaDeclarada").GetDecimal();
                        var numeroConta = root.GetProperty("conta").GetProperty("numeroConta").GetString() ?? string.Empty;
                        var valorLimite = renda >= 10000m ? renda * 2m : renda * 0.5m;
                        var statusCredito = "APROVADO";

                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<BancoContext>();
                        var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.DocumentoPrincipal == documento, stoppingToken);
                        if (cliente is null)
                        {
                            continue;
                        }

                        var limiteCredito = new Domain.LimiteCredito
                        {
                            ClienteId = cliente.Id,
                            ContaId = cliente.Id,
                            RendaDeclarada = renda,
                            LimiteLiberado = valorLimite,
                            StatusCredito = StatusCredito.APROVADO
                        };

                        db.LimitesCredito.Add(limiteCredito);
                        await db.SaveChangesAsync(stoppingToken);

                        var payload = new
                        {
                            evento = "CREDITO_CONCEDIDO",
                            dataEnvio = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            documento,
                            numeroConta,
                            rendaDeclarada = renda,
                            limiteCartaoLiberado = valorLimite,
                            statusCredito
                        };

                        await _producer.ProduceAsync("credito-concedido", payload);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue processing
                        System.Diagnostics.Debug.WriteLine($"CreditApprovalConsumer error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreditApprovalConsumer fatal error: {ex.Message}");
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
