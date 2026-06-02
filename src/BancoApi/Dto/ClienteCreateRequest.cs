using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using BancoApi.Domain;

namespace BancoApi.Dto;

public class ClienteCreateRequest
{
    [JsonPropertyName("tipoPessoa")]
    public TipoPessoa TipoPessoa { get; set; }

    [JsonPropertyName("nome")]
    [Required]
    public string Nome { get; set; } = null!;

    [JsonPropertyName("documento")]
    [Required]
    public string Documento { get; set; } = null!;

    [JsonPropertyName("documentoSecundario")]
    public string? DocumentoSecundario { get; set; }

    [JsonPropertyName("dataNascimentoFundacao")]
    public DateTime DataNascimentoFundacao { get; set; }

    [JsonPropertyName("email")]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [JsonPropertyName("telefone")]
    [Required]
    public string Telefone { get; set; } = null!;

    [JsonPropertyName("logradouro")]
    [Required]
    public string Logradouro { get; set; } = null!;

    [JsonPropertyName("numero")]
    [Required]
    public string Numero { get; set; } = null!;

    [JsonPropertyName("cep")]
    [Required]
    public string Cep { get; set; } = null!;

    [JsonPropertyName("cidade")]
    [Required]
    public string Cidade { get; set; } = null!;

    [JsonPropertyName("uf")]
    [Required]
    public string Uf { get; set; } = null!;

    [JsonPropertyName("rendaMensal")]
    public decimal RendaMensal { get; set; }

    [JsonPropertyName("numeroAgencia")]
    [Required]
    public string NumeroAgencia { get; set; } = null!;

    [JsonPropertyName("tipoConta")]
    public TipoConta TipoConta { get; set; }

    [JsonPropertyName("senha")]
    [Required]
    public string Senha { get; set; } = null!;
}
