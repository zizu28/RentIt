using BCrypt.Net;
using RentIt.Modules.Identity.Application.Abstractions;

namespace RentIt.Modules.Identity.Infrastructure.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int DefaultWorkFactor = 12;

    public string HashPassword(string password)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt(DefaultWorkFactor);
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
