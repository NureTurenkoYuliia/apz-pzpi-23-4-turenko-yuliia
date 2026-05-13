namespace Application.Abstractions;

public interface IPasswordHasher
{
    string HashPassword(string password, string salt);
    string GenerateSalt();
}
