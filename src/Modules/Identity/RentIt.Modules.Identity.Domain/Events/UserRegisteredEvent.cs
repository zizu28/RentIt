using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Identity.Domain.Events;

/// <summary>
/// Event raised when a new user registers
/// </summary>
public sealed record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string PhoneNumber,
    string Role
) : DomainEvent;
