namespace RentIt.Shared.Contracts.Identity;

/// <summary>
/// Login request
/// </summary>
public sealed record LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
