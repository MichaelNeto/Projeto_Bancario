using BancoApi.Data;
using BancoApi.Dto;
using BancoApi.Domain;
using BancoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Controllers;

[ApiController]
[Route("api/transacoes")]
[Authorize]
public class TransacoesController : ControllerBase
{
    private readonly BancoContext _db;
    private readonly KafkaProducerService _producer;

    public TransacoesController(BancoContext db, KafkaProducerService producer)
    {
        _db = db;
        _producer = producer;
    }

    [HttpPost("deposito")]
    public async Task<IActionResult> Depositar([FromBody] DepositoRequest request)
    {
        // Validar se o valor é maior que zero
        if (request.Valor <= 0)
        {
            return BadRequest(new { mensagem = "Valor do depósito deve ser maior que zero" });
        }

        // Extrair o ID do cliente do token JWT
        var clienteId = int.Parse(User.FindFirst("clienteId")?.Value ?? "0");
        
        // Buscar o cliente no banco
        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == clienteId);
        if (cliente is null)
        {
            return BadRequest(new { mensagem = "Conta de destino inválida" });
        }

        // Validar se a conta está ativa
        if (cliente.StatusCliente != StatusCliente.ATIVO)
        {
            return BadRequest(new { mensagem = "Conta inativa ou bloqueada" });
        }

        // Iniciar transação
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            // Atualizar saldo do cliente
            cliente.Saldo += request.Valor;

            // Criar registro no extrato
            var transacao = new Transacao
            {
                ContaOrigemId = cliente.Id,  // A origem é o próprio cliente (depósito recebido)
                ContaDestinoId = null,       // Não há destinatário específico
                Valor = request.Valor,
                Status = StatusTransacao.Concluido,
                Tipo = TipoTransacao.DEPOSITO_RECEBIDO,
                Observacao = "Depósito em conta",
                DataHora = DateTime.UtcNow
            };

            _db.Transacoes.Add(transacao);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            // Produzir mensagem Kafka
            var evento = new
            {
                evento = "DEPOSITO_CONCLUIDO",
                idTransacao = transacao.Id.ToString(),
                dataHora = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                cliente = new
                {
                    id = cliente.Id,
                    nome = cliente.Nome,
                    documento = cliente.DocumentoPrincipal,
                    agencia = cliente.NumeroAgencia,
                    conta = cliente.NumeroConta,
                    email = cliente.Email
                },
                financeiro = new
                {
                    valor = request.Valor,
                    saldoAnterior = cliente.Saldo - request.Valor,
                    saldoAtual = cliente.Saldo
                }
            };

            await _producer.ProduceAsync("deposito-concluido", evento);

            return Ok(new 
            { 
                mensagem = "Depósito realizado com sucesso", 
                idTransacao = transacao.Id,
                saldoAtual = cliente.Saldo,
                valorDepositado = request.Valor
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { mensagem = "Erro ao processar o depósito", erro = ex.Message });
        }
    }
}
