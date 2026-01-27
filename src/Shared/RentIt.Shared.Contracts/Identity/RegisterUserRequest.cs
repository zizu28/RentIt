namespace RentIt.Shared.Contracts.Identity;

/// <summary>
/// Register user request
/// </summary>
public sealed record RegisterUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}
