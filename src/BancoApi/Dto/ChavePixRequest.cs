namespace BancoApi.Dto;

public class ChavePixRequest
{
    public string Tipo { get; set; } = null!;  // CPF, EMAIL, TELEFONE, ALEATORIA
    public string? Valor { get; set; }  // Pode ser nulo se for ALEATORIA
}

public class ChavePixDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = null!;
    public string Valor { get; set; } = null!;
    public DateTime DataCadastro { get; set; }
    public bool Ativa { get; set; }
}
