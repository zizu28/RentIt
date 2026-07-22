using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Identity.IntegrationEvents;

/// <summary>
/// Published when a suspended user account is reactivated.
/// Consumed by: Properties (re-publish listings), Messaging (re-enable channels)
/// </summary>
public sealed record UserReactivatedIntegrationEvent(
    Guid UserId,
    string Email
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
