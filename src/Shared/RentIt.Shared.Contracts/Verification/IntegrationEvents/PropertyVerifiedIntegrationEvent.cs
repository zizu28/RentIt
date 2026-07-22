using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Verification.IntegrationEvents;

/// <summary>
/// Published when a property listing passes verification checks.
/// Consumed by: Properties (move listing from Draft to Published)
/// </summary>
public sealed record PropertyVerifiedIntegrationEvent(
    Guid PropertyId,
    Guid LandlordId
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
