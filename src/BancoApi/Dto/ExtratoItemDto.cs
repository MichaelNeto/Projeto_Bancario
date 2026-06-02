using System.Text.Json.Serialization;

namespace BancoApi.Dto;

public class ExtratoItemDto
{
    [JsonPropertyName("dataHora")]
    public DateTime DataHora { get; set; }

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = null!;

    [JsonPropertyName("nomeContraparte")]
    public string NomeContraparte { get; set; } = null!;

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
}
