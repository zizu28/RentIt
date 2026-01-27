namespace RentIt.Shared.Contracts.Identity;

/// <summary>
/// Login response with JWT tokens
/// </summary>
public sealed record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}
