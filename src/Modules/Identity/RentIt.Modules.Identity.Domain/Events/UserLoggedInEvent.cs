using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Identity.Domain.Events;

/// <summary>
/// Event raised when user successfully logs in
/// </summary>
public sealed record UserLoggedInEvent(
    Guid UserId,
    string Email,
    DateTime LoginAt
) : DomainEvent;
