using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoApi.Domain;

public class Cliente
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    [Required]
    [MaxLength(14)]
    public TipoPessoa TipoPessoa { get; set; }

    [Required]
    [MaxLength(200)]
    public string DocumentoPrincipal { get; set; } = null!;

    [MaxLength(50)]
    public string? DocumentoSecundario { get; set; }

    public DateTime DataNascimentoFundacao { get; set; }

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Telefone { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Logradouro { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string NumeroEndereco { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    public string Cep { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Cidade { get; set; } = null!;

    [Required]
    [MaxLength(2)]
    public string Uf { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal RendaMensal { get; set; }

    [Required]
    [MaxLength(4)]
    public string NumeroAgencia { get; set; } = null!;

    [Required]
    [MaxLength(12)]
    public string NumeroConta { get; set; } = null!;

    public TipoConta TipoConta { get; set; }

    [Required]
    [MaxLength(36)]
    public string ChavePix { get; set; } = null!;

    [Required]
    public StatusCliente StatusCliente { get; set; } = StatusCliente.ATIVO;

    [Required]
    [MaxLength(200)]
    public string SenhaHash { get; set; } = null!;

    public int TentativasFalhas { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Saldo { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public bool EstaAtivo => StatusCliente == StatusCliente.ATIVO;
}
