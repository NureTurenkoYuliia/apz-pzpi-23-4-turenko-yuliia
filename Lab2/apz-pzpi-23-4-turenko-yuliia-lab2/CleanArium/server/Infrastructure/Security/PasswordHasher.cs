using Application.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string GenerateSalt()
    {
        var bytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToBase64String(bytes);
    }

    public string HashPassword(string password, string salt)
    {
        var bytes = Encoding.UTF8.GetBytes(password + salt);
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}
