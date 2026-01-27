namespace RentIt.Modules.Identity.Application.Abstractions;

/// <summary>
/// Password hashing service
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
