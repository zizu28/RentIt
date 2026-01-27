namespace RentIt.Modules.Identity.Application.Abstractions;

/// <summary>
/// JWT token generation service
/// </summary>
public interface IJwtTokenGenerator
{
    string GenerateAccessToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
}
