using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoApi.Domain;

public class ChavePix
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    [Required]
    public TipoChavePix Tipo { get; set; }

    [Required]
    [MaxLength(255)]
    public string Valor { get; set; } = null!;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    [Required]
    public bool Ativa { get; set; } = true;
}
