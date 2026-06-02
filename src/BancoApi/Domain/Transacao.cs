using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoApi.Domain;

public class Transacao
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime DataHora { get; set; } = DateTime.UtcNow;

    public TipoTransacao Tipo { get; set; }

    public StatusTransacao Status { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }

    public int ContaOrigemId { get; set; }
    public Cliente? ContaOrigem { get; set; }

    public int? ContaDestinoId { get; set; }
    public Cliente? ContaDestino { get; set; }

    [MaxLength(200)]
    public string? Observacao { get; set; }
}
