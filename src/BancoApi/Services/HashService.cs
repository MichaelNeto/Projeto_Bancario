using BCrypt.Net;

namespace BancoApi.Services;

public class HashService
{
    public string HashPassword(string senha)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(senha);
    }

    public bool VerifyPassword(string senha, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(senha, hash);
    }
}
