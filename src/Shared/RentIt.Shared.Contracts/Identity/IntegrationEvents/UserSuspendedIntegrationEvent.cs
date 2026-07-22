using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Identity.IntegrationEvents;

/// <summary>
/// Published when an admin suspends a user account.
/// Consumed by: Bookings (cancel active bookings), Properties (delist listings), Messaging (disable channels)
/// </summary>
public sealed record UserSuspendedIntegrationEvent(
    Guid UserId,
    string Email,
    string Reason
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
