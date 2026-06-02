using System.Text.RegularExpressions;
using BancoApi.Domain;
using Microsoft.Extensions.Configuration;

namespace BancoApi.Services;

public class ValidationService
{
    private readonly string[] _agenciasValidas;

    public ValidationService(IConfiguration configuration)
    {
        _agenciasValidas = configuration.GetSection("ValidAgencias").Get<string[]>() ?? new[] { "0001", "0002", "0003" };
    }

    public bool ValidarNomeOuRazaoSocial(string texto)
    {
        return !string.IsNullOrWhiteSpace(texto) && Regex.IsMatch(texto.Trim(), @"^[\p{L} ]+$");
    }

    public bool ValidarEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public bool ValidarCep(string cep)
    {
        return !string.IsNullOrWhiteSpace(cep) && Regex.IsMatch(cep.Trim(), @"^[0-9]{8}$");
    }

    public bool ValidarTelefone(string telefone)
    {
        return !string.IsNullOrWhiteSpace(telefone) && Regex.IsMatch(telefone.Trim(), @"^[0-9]{10,11}$");
    }

    public bool ValidarDocumentoPrincipal(TipoPessoa tipoPessoa, string documento)
    {
        if (tipoPessoa == TipoPessoa.PF)
        {
            return !string.IsNullOrWhiteSpace(documento) && Regex.IsMatch(documento.Trim(), @"^[0-9]{11}$");
        }

        return !string.IsNullOrWhiteSpace(documento) && Regex.IsMatch(documento.Trim(), @"^[0-9]{14}$");
    }

    public bool ValidarDataNascimentoFundacao(TipoPessoa tipoPessoa, DateTime data)
    {
        if (tipoPessoa == TipoPessoa.PF)
        {
            var idade = DateTime.UtcNow.Year - data.Year;
            if (data.Date > DateTime.UtcNow.AddYears(-idade))
            {
                idade--;
            }

            return idade >= 18;
        }

        return true;
    }

    public bool ValidarAgencia(string agencia)
    {
        return _agenciasValidas.Contains(agencia);
    }

    public bool ValidarSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length != 6 || !senha.All(char.IsDigit))
        {
            return false;
        }

        if (senha.Distinct().Count() == 1)
        {
            return false;
        }

        if (senha == "123456" || senha == "234567" || senha == "345678" || senha == "456789" || senha == "012345" || senha == "654321" || senha == "543210")
        {
            return false;
        }

        bool isAscending = true;
        bool isDescending = true;

        for (var i = 1; i < senha.Length; i++)
        {
            var previous = senha[i - 1] - '0';
            var current = senha[i] - '0';
            if (current != previous + 1)
            {
                isAscending = false;
            }
            if (current != previous - 1)
            {
                isDescending = false;
            }
        }

        if (isAscending || isDescending)
        {
            return false;
        }

        return true;
    }

    public bool ValidarChavePix(string chave)
    {
        var valor = chave?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(valor)) return false;

        if (Guid.TryParse(valor, out _)) return true;
        if (ValidarEmail(valor)) return true;
        if (Regex.IsMatch(valor, "^[0-9]{11}$")) return true; // CPF ou celular
        if (Regex.IsMatch(valor, "^[A-Za-z0-9]{14}$")) return true; // CNPJ ou chave alfanumérica

        return false;
    }
}
