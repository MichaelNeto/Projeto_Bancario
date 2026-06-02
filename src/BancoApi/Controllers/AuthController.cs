using BancoApi.Data;
using BancoApi.Dto;
using BancoApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly BancoContext _db;
    private readonly HashService _hashService;
    private readonly JwtService _jwtService;

    public AuthController(BancoContext db, HashService hashService, JwtService jwtService)
    {
        _db = db;
        _hashService = hashService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.DocumentoPrincipal == request.Documento);
        if (cliente is null)
        {
            return Unauthorized(new { mensagem = "Usuário ou senha inválidos" });
        }

        if (cliente.StatusCliente == Domain.StatusCliente.BLOQUEADO)
        {
            return Unauthorized(new { mensagem = "Acesso negado. Usuário bloqueado" });
        }

        if (!_hashService.VerifyPassword(request.Senha, cliente.SenhaHash))
        {
            cliente.TentativasFalhas += 1;
            if (cliente.TentativasFalhas >= 3)
            {
                cliente.StatusCliente = Domain.StatusCliente.BLOQUEADO;
                await _db.SaveChangesAsync();
                return Unauthorized(new { mensagem = "Conta bloqueada por excesso de tentativas. Contate o suporte" });
            }

            await _db.SaveChangesAsync();
            return Unauthorized(new { mensagem = "Usuário ou senha inválidos" });
        }

        cliente.TentativasFalhas = 0;
        await _db.SaveChangesAsync();

        var token = _jwtService.GenerateToken(cliente.Id, cliente.NumeroAgencia, cliente.NumeroConta);
        return Ok(new { token });
    }
}
