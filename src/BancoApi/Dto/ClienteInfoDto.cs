namespace BancoApi.Dto;

public class ClienteInfoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string NumeroAgencia { get; set; } = null!;
    public string NumeroConta { get; set; } = null!;
    public string ChavePix { get; set; } = null!;
    public decimal Saldo { get; set; }
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;
}
