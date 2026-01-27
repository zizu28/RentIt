using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Identity.Domain.Events;

/// <summary>
/// Event raised when user verifies their email
/// </summary>
public sealed record EmailVerifiedEvent(
    Guid UserId,
    string Email
) : DomainEvent;
