namespace RentIt.Shared.DTOs.Identity;

public sealed record RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
