using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Identity.Domain.Events;

/// <summary>
/// Event raised when user's phone number is verified
/// </summary>
public sealed record PhoneNumberVerifiedEvent(
    Guid UserId,
    string PhoneNumber
) : DomainEvent;
