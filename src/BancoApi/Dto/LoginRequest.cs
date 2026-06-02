using System.Text.Json.Serialization;

namespace BancoApi.Dto;

public class LoginRequest
{
    [JsonPropertyName("documento")]
    public string Documento { get; set; } = null!;

    [JsonPropertyName("senha")]
    public string Senha { get; set; } = null!;
}
