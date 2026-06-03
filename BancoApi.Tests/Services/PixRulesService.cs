namespace BancoApi.Services;

public class PixRulesService
{
    public bool ValidarSaldo(decimal saldo, decimal valor)
    {
        return saldo >= valor;
    }

    public bool ValidarValor(decimal valor)
    {
        return valor > 0;
    }
}
