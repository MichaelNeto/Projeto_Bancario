using BancoApi.Data;
using BancoApi.Domain;
using BancoApi.Dto;
using BancoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly BancoContext _db;
    private readonly ValidationService _validation;
    private readonly HashService _hashService;
    private readonly KafkaProducerService _producer;

    public ClientesController(BancoContext db, ValidationService validation, HashService hashService, KafkaProducerService producer)
    {
        _db = db;
        _validation = validation;
        _hashService = hashService;
        _producer = producer;
    }

    [HttpPost]
    public async Task<IActionResult> CriarCliente([FromBody] ClienteCreateRequest request)
    {
        try
        {
           if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value != null && kvp.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors
                            .Select(e => e.ErrorMessage)
                            .ToArray()
                    );

                return BadRequest(new Dto.ErrorResponse
                {
                    Status = 400,
                    Message = "Dados inválidos",
                    Errors = errors
                });
            }
            if (!_validation.ValidarNomeOuRazaoSocial(request.Nome))
            {
                return BadRequest(new Dto.ErrorResponse { Status = 400, Message = "Nome ou razão social inválido" });
            }

            if (!_validation.ValidarDocumentoPrincipal(request.TipoPessoa, request.Documento))
            {
                return BadRequest(new Dto.ErrorResponse { Status = 400, Message = "Documento inválido" });
            }

            if (await _db.Clientes.AnyAsync(c => c.DocumentoPrincipal == request.Documento))
            {
                return Conflict(new Dto.ErrorResponse { Status = 409, Message = "Documento já cadastrado no sistema" });
            }

            if (!_validation.ValidarEmail(request.Email))
            {
                return BadRequest(new Dto.ErrorResponse { Status = 400, Message = "E-mail inválido" });
            }

            if (!_validation.ValidarTelefone(request.Telefone))
            {
                return BadRequest(new Dto.ErrorResponse { Status = 400, Message = "Telefone inválido" });
            }

            if (!_validation.ValidarCep(request.Cep))
            {
                return BadRequest(new Dto.ErrorResponse { Status = 400, Message = "CEP inválido" });
            }

            if (!_validation.ValidarAgencia(request.NumeroAgencia))
            {
                return BadRequest(new Dto.ErrorResponse { Status = 400, Message = "Agência inválida" });
            }

            if (request.RendaMensal <= 0)
            {
                return BadRequest(new Dto.ErrorResponse { Status = 400, Message = "Renda mensal deve ser um valor maior que zero" });
            }

            if (!_validation.ValidarDataNascimentoFundacao(request.TipoPessoa, request.DataNascimentoFundacao))
            {
                return BadRequest(new Dto.ErrorResponse { Status = 400, Message = "Cliente deve ter pelo menos 18 anos" });
            }

            if (!request.Senha.All(char.IsDigit) || request.Senha.Length != 6)
            {
                return BadRequest(new Dto.ErrorResponse { Status = 400, Message = "A senha deve conter exatamente 6 números" });
            }

            if (!_validation.ValidarSenha(request.Senha))
            {
                return BadRequest(new Dto.ErrorResponse { Status = 400, Message = "Senha muito fraca. Evite números sequenciais ou repetidos" });
            }

            var cliente = new Cliente
            {
                TipoPessoa = request.TipoPessoa,
                Nome = request.Nome.Trim(),
                DocumentoPrincipal = request.Documento.Trim(),
                DocumentoSecundario = request.DocumentoSecundario?.Trim(),
                DataNascimentoFundacao = request.DataNascimentoFundacao,
                Email = request.Email.Trim(),
                Telefone = request.Telefone.Trim(),
                Logradouro = request.Logradouro.Trim(),
                NumeroEndereco = request.Numero.Trim(),
                Cep = request.Cep.Trim(),
                Cidade = request.Cidade.Trim(),
                Uf = request.Uf.Trim().ToUpper(),
                RendaMensal = request.RendaMensal,
                NumeroAgencia = request.NumeroAgencia.Trim(),
                TipoConta = request.TipoConta,
                NumeroConta = await GerarNumeroContaUnicoAsync(),
                ChavePix = Guid.NewGuid().ToString(),
                SenhaHash = _hashService.HashPassword(request.Senha),
                StatusCliente = StatusCliente.ATIVO,
                Saldo = 0m
            };

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();

            var evento = new
            {
                evento = "CLIENTE_CADASTRADO",
                dataEnvio = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                cliente = new
                {
                    tipoPessoa = cliente.TipoPessoa.ToString(),
                    documento = cliente.DocumentoPrincipal,
                    nome = cliente.Nome,
                    email = cliente.Email,
                    telefone = cliente.Telefone,
                    rendaDeclarada = cliente.RendaMensal
                },
                conta = new
                {
                    numeroAgencia = cliente.NumeroAgencia,
                    numeroConta = cliente.NumeroConta,
                    tipo = cliente.TipoConta.ToString(),
                    chavePixAleatoria = cliente.ChavePix
                },
                usuario = new
                {
                    login = cliente.DocumentoPrincipal,
                    statusLogin = "ATIVO"
                }
            };

            try
            {
                await _producer.ProduceAsync("cliente-cadastrado", evento);
            }
            catch (Exception kafkaError)
            {
                System.Diagnostics.Debug.WriteLine($"Kafka error (non-blocking): {kafkaError.Message}");
                // Não falha o cadastro se Kafka estiver offline
            }

            return CreatedAtAction(nameof(CriarCliente), new { id = cliente.Id }, new { cliente.Id, cliente.DocumentoPrincipal, cliente.NumeroAgencia, cliente.NumeroConta });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CriarCliente error: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, new Dto.ErrorResponse { Status = 500, Message = $"Erro ao criar cliente: {ex.Message}" });
        }
    }

    private async Task<string> GerarNumeroContaUnicoAsync()
    {
        while (true)
        {
            var numero = $"{Random.Shared.Next(0, 999999):D6}-{Random.Shared.Next(0, 10)}";
            if (!await _db.Clientes.AnyAsync(c => c.NumeroConta == numero))
            {
                return numero;
            }
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> ObterDadosCliente()
    {
        try
        {
            var clienteIdClaim = User.FindFirst("clienteId");
            if (clienteIdClaim == null || !int.TryParse(clienteIdClaim.Value, out var clienteId))
            {
                return Unauthorized(new { mensagem = "ClienteId não encontrado no token" });
            }

            var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == clienteId);

            if (cliente is null)
            {
                return NotFound(new { mensagem = "Cliente não encontrado" });
            }

            var clienteInfo = new ClienteInfoDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                NumeroAgencia = cliente.NumeroAgencia,
                NumeroConta = cliente.NumeroConta,
                ChavePix = cliente.ChavePix,
                Saldo = cliente.Saldo,
                Email = cliente.Email,
                Telefone = cliente.Telefone
            };

            return Ok(clienteInfo);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ObterDadosCliente error: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, new Dto.ErrorResponse { Status = 500, Message = $"Erro ao obter dados do cliente: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListarClientes()
    {
        try
        {
            var clientes = await _db.Clientes.ToListAsync();
            return Ok(new { total = clientes.Count, clientes = clientes });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ListarClientes error: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, new Dto.ErrorResponse { Status = 500, Message = $"Erro ao listar clientes: {ex.Message}" });
        }
    }
}
