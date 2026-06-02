using BancoApi.Data;
using BancoApi.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Controllers;

[ApiController]
[Route("api/extrato")]
[Authorize]
public class ExtratoController : ControllerBase
{
    private readonly BancoContext _db;

    public ExtratoController(BancoContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Consultar([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
    {
        var clienteId = int.Parse(User.FindFirst("clienteId")?.Value ?? "0");

        var dataFim = fim?.Date.AddDays(1).AddTicks(-1) ?? DateTime.UtcNow;
        var dataInicio = inicio?.Date ?? dataFim.AddDays(-30);

        if (dataFim < dataInicio)
        {
            return BadRequest(new { mensagem = "Data inicial não pode ser maior que a data final" });
        }

        if ((dataFim - dataInicio).TotalDays > 90)
        {
            return BadRequest(new { mensagem = "O período máximo para consulta do extrato é de 90 dias" });
        }

        var transacoes = await _db.Transacoes
            .Include(t => t.ContaOrigem)
            .Include(t => t.ContaDestino)
            .Where(t => t.DataHora >= dataInicio && t.DataHora <= dataFim &&
                        (t.ContaOrigemId == clienteId || t.ContaDestinoId == clienteId))
            .OrderByDescending(t => t.DataHora)
            .ToListAsync();

        var resultado = transacoes.Select(t =>
        {
            var ehSaida = t.Tipo == Domain.TipoTransacao.PIX_ENVIADO && t.ContaOrigemId == clienteId;
            var nomeContraparte = ehSaida
                ? t.ContaDestino?.Nome ?? "Recebedor desconhecido"
                : t.ContaOrigem?.Nome ?? "Pagador desconhecido";

            return new ExtratoItemDto
            {
                DataHora = t.DataHora,
                Tipo = t.Tipo.ToString(),
                NomeContraparte = nomeContraparte,
                Valor = ehSaida ? -t.Valor : t.Valor,
                Status = t.Status.ToString()
            };
        }).ToList();

        return Ok(resultado);
    }
}
