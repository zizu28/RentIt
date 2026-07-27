namespace RentIt.Shared.DTOs.Identity;

/// <summary>
/// User DTO
/// </summary>
public sealed record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsEmailVerified { get; init; }
    public bool IsPhoneVerified { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
}
