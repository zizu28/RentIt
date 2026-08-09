using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Verification.IntegrationEvents;

/// <summary>
/// Published when a property listing fails verification checks.
/// Consumed by: Properties (keep in Draft), Messaging (notify Host with rejection reason)
/// </summary>
public sealed record PropertyRejectedIntegrationEvent(
    Guid PropertyId,
    Guid HostId,
    string Reason
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
