namespace RentIt.Shared.Contracts.Identity;

/// <summary>
/// Refresh token request
/// </summary>
public sealed record RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
