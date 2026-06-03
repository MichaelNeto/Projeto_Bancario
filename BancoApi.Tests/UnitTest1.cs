using BancoApi.Services;
using FluentAssertions;

namespace BancoApi.Tests.Services;

public class PixRulesServiceTests
{
    [Fact]
    public void Deve_Permitir_Pix_Com_Saldo_Suficiente()
    {
        var service = new PixRulesService();

        var resultado =
            service.ValidarSaldo(1000, 100);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void Deve_Bloquear_Pix_Com_Saldo_Insuficiente()
    {
        var service = new PixRulesService();

        var resultado =
            service.ValidarSaldo(100, 1000);

        resultado.Should().BeFalse();
    }
}
