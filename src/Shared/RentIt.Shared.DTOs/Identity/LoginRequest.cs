namespace RentIt.Shared.DTOs.Identity;

/// <summary>
/// Login request
/// </summary>
public sealed record LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
