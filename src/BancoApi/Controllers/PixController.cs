using BancoApi.Data;
using BancoApi.Dto;
using BancoApi.Domain;
using BancoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Controllers;

[ApiController]
[Route("api/pix")]
public class PixController : ControllerBase
{
    private readonly BancoContext _db;
    private readonly KafkaProducerService _producer;

    public PixController(BancoContext db, KafkaProducerService producer)
    {
        _db = db;
        _producer = producer;
    }

    [HttpPost("transferir")]
    [Authorize]
    public async Task<IActionResult> Transferir([FromBody] PixTransferRequest request)
    {
        if (request.Valor <= 0)
        {
            return BadRequest(new { mensagem = "Valor deve ser maior que zero" });
        }

        var clienteId = int.Parse(User.FindFirst("clienteId")?.Value ?? "0");
        var clienteOrigem = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == clienteId);
        if (clienteOrigem is null || !clienteOrigem.EstaAtivo)
        {
            return BadRequest(new { mensagem = "Conta de origem inválida ou inativa" });
        }

        var horaAtual = DateTime.UtcNow.Hour;
        var limiteHorario = horaAtual >= 20 || horaAtual <= 5 ? 1000m : 5000m;

        if (request.Valor > limiteHorario)
        {
            return BadRequest(new { mensagem = horaAtual >= 20 || horaAtual <= 5 ? "Valor acima do limite permitido para o horário noturno" : "Valor acima do limite permitido para o horário diurno" });
        }

        if (request.Valor > clienteOrigem.Saldo)
        {
            return BadRequest(new { mensagem = "Saldo insuficiente para realizar a transação" });
        }

        var chavePix = await _db.ChavesPix
            .Include(c => c.Cliente)
            .FirstOrDefaultAsync(c =>
                c.Valor == request.ChaveDestino &&
                c.Ativa);

        if (chavePix is null)
        {
            return NotFound(new { mensagem = "Chave Pix não encontrada" });
        }

        var clienteDestino = chavePix.Cliente;

        if (clienteDestino is null)
        {
            return NotFound(new { mensagem = "Cliente destino não encontrado" });
        }


if (clienteDestino is null)
{
    return NotFound(new { mensagem = "Cliente destino não encontrado" });
}

// Impedir PIX para a própria conta
if (clienteDestino.Id == clienteOrigem.Id)
{
    return BadRequest(new
    {
        mensagem = "Não é permitido enviar PIX para a própria conta"
    });
}

clienteOrigem.Saldo -= request.Valor;
        

        await using var transaction = await _db.Database.BeginTransactionAsync();
        clienteOrigem.Saldo -= request.Valor;

        var transacao = new Transacao
        {
            ContaOrigemId = clienteOrigem.Id,
            ContaDestinoId = clienteDestino.Id,
            Valor = request.Valor,
            Status = StatusTransacao.Processando,
            Tipo = TipoTransacao.PIX_ENVIADO,
            Observacao = "PIX em processamento"
        };

        _db.Transacoes.Add(transacao);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        var evento = new
        {
            evento = "PIX_SOLICITADO",
            idTransacao = transacao.Id.ToString(),
            dataHora = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            origem = new
            {
                documento = clienteOrigem.DocumentoPrincipal,
                agencia = clienteOrigem.NumeroAgencia,
                conta = clienteOrigem.NumeroConta
            },
            destino = new
            {
                chavePix = clienteDestino.ChavePix,
                tipoChave = ClassificarTipoChave(request.ChaveDestino)
            },
            financeiro = new
            {
                valor = request.Valor
            }
        };

        await _producer.ProduceAsync("pix-solicitado", evento);
        return Ok(new { mensagem = "PIX solicitado com sucesso", idTransacao = transacao.Id });
    }

    private static string ClassificarTipoChave(string chave)
    {
        if (Guid.TryParse(chave, out _)) return "ALEATORIA";
        if (chave.Contains("@")) return "EMAIL";
        if (chave.Length == 11 && chave.All(char.IsDigit)) return "CELULAR";
        if (chave.Length == 14) return "CNPJ";
        return "DESCONHECIDO";
    }

    [HttpPost("chaves")]
    [Authorize]
    public async Task<IActionResult> CadastrarChavePix([FromBody] ChavePixRequest request)
    {
        // Extrair ID do cliente do token
        var clienteId = int.Parse(User.FindFirst("clienteId")?.Value ?? "0");
        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == clienteId);

        if (cliente is null || cliente.StatusCliente != StatusCliente.ATIVO)
        {
            return BadRequest(new { mensagem = "Conta inválida ou inativa" });
        }

        // Validar tipo de chave
        if (!Enum.TryParse<TipoChavePix>(request.Tipo, ignoreCase: true, out var tipoChave))
        {
            return BadRequest(new { mensagem = "Tipo de chave inválido (CPF, EMAIL, TELEFONE, ALEATORIA)" });
        }

        // Validar limite de chaves
        var chavesExistentes = await _db.ChavesPix
            .Where(c => c.ClienteId == clienteId && c.Ativa)
            .CountAsync();

        int limiteChaves = cliente.TipoPessoa == TipoPessoa.PF ? 5 : 20;

        if (chavesExistentes >= limiteChaves)
        {
            return BadRequest(new { mensagem = $"Limite de {limiteChaves} chaves atingido para este tipo de conta" });
        }

        string valorChave = request.Valor ?? "";

        // Validações específicas por tipo
        if (tipoChave == TipoChavePix.CPF)
        {
            // CPF deve ser igual ao do cliente
            if (cliente.TipoPessoa != TipoPessoa.PF)
            {
                return BadRequest(new { mensagem = "Apenas PF podem registrar CPF como chave Pix" });
            }

            if (valorChave.Replace(".", "").Replace("-", "") != cliente.DocumentoPrincipal)
            {
                return BadRequest(new { mensagem = "CPF deve ser igual ao documento da conta" });
            }
        }
        else if (tipoChave == TipoChavePix.EMAIL)
        {
            if (string.IsNullOrWhiteSpace(valorChave))
            {
                return BadRequest(new { mensagem = "E-mail é obrigatório para este tipo de chave" });
            }

            // Validar formato de email
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
            if (!emailRegex.IsMatch(valorChave))
            {
                return BadRequest(new { mensagem = "E-mail inválido" });
            }

            valorChave = valorChave.ToLower();
        }
        else if (tipoChave == TipoChavePix.TELEFONE)
        {
            if (string.IsNullOrWhiteSpace(valorChave))
            {
                return BadRequest(new { mensagem = "Telefone é obrigatório para este tipo de chave" });
            }

            // Validar formato (DDI + DDD + número)
            var telefoneLimpo = valorChave.Replace("+", "").Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            if (telefoneLimpo.Length < 10 || !telefoneLimpo.All(char.IsDigit))
            {
                return BadRequest(new { mensagem = "Telefone inválido. Use formato: +5511999999999" });
            }

            valorChave = "+" + telefoneLimpo;
        }
        else if (tipoChave == TipoChavePix.ALEATORIA)
        {
            // Gerar UUID aleatória
            valorChave = Guid.NewGuid().ToString();
        }

        // Verificar unicidade
        var chaveExistente = await _db.ChavesPix
            .FirstOrDefaultAsync(c => c.Valor == valorChave);

        if (chaveExistente is not null)
        {
            return Conflict(new { mensagem = "Esta chave Pix já está registrada em outra conta" });
        }

        // Criar nova chave
        var novachave = new ChavePix
        {
            ClienteId = clienteId,
            Tipo = tipoChave,
            Valor = valorChave,
            DataCadastro = DateTime.UtcNow,
            Ativa = true
        };

        _db.ChavesPix.Add(novachave);
        await _db.SaveChangesAsync();

        // Publicar evento no Kafka
        var evento = new
        {
            evento = "CHAVE_PIX_CADASTRADA",
            idChave = novachave.Id.ToString(),
            dataHora = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            cliente = new
            {
                id = cliente.Id,
                nome = cliente.Nome,
                documento = cliente.DocumentoPrincipal,
                email = cliente.Email
            },
            chave = new
            {
                tipo = novachave.Tipo.ToString(),
                valor = tipoChave == TipoChavePix.ALEATORIA ? novachave.Valor : "***" // Mascarar valor sensível
            }
        };

        try
        {
            await _producer.ProduceAsync("chave-pix-cadastrada", evento);
        }
        catch (Exception kafkaError)
        {
            System.Diagnostics.Debug.WriteLine($"Kafka error (non-blocking): {kafkaError.Message}");
        }

        return Ok(new
        {
            mensagem = "Chave Pix cadastrada com sucesso",
            chave = new ChavePixDto
            {
                Id = novachave.Id,
                Tipo = novachave.Tipo.ToString(),
                Valor = tipoChave == TipoChavePix.ALEATORIA ? novachave.Valor : "***",
                DataCadastro = novachave.DataCadastro,
                Ativa = novachave.Ativa
            }
        });
    }

    [HttpGet("chaves")]
    [Authorize]
    public async Task<IActionResult> ListarChavesPix()
    {
        var clienteId = int.Parse(User.FindFirst("clienteId")?.Value ?? "0");

        var chaves = await _db.ChavesPix
            .Where(c => c.ClienteId == clienteId && c.Ativa)
            .OrderByDescending(c => c.DataCadastro)
            .ToListAsync();

        var resultado = chaves.Select(c => new ChavePixDto
        {
            Id = c.Id,
            Tipo = c.Tipo.ToString(),
            Valor = c.Tipo == TipoChavePix.ALEATORIA ? c.Valor : "***",
            DataCadastro = c.DataCadastro,
            Ativa = c.Ativa
        }).ToList();

        return Ok(resultado);
    }

    [HttpDelete("chaves/{id}")]
    [Authorize]
    public async Task<IActionResult> DeletarChavePix(Guid id)
    {
        var clienteId = int.Parse(User.FindFirst("clienteId")?.Value ?? "0");

        var chave = await _db.ChavesPix
            .FirstOrDefaultAsync(c => c.Id == id && c.ClienteId == clienteId);

        if (chave is null)
        {
            return NotFound(new { mensagem = "Chave Pix não encontrada" });
        }

        chave.Ativa = false;
        await _db.SaveChangesAsync();

        return Ok(new { mensagem = "Chave Pix removida com sucesso" });
    }
}
