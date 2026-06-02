using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoApi.Domain;

public class LimiteCredito
{
    [Key]
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int ContaId { get; set; }
    public Cliente? Conta { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RendaDeclarada { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LimiteLiberado { get; set; }

    public StatusCredito StatusCredito { get; set; }

    public DateTime DataConcessao { get; set; } = DateTime.UtcNow;
}
