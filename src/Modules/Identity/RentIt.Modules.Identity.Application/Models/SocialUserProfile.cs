namespace RentIt.Modules.Identity.Application.Models;

public sealed record SocialUserProfile(
    string Provider,
    string ProviderId,
    string Email,
    string? FirstName,
    string? LastName,
    string? ProfileImageUrl = null,
    string? Address = null
);