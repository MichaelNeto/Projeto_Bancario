using System.Text.Json.Serialization;

namespace BancoApi.Dto;

public class PixTransferRequest
{
    [JsonPropertyName("chaveDestino")]
    public string ChaveDestino { get; set; } = null!;

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }
}
