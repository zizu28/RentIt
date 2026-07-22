using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Properties.IntegrationEvents;

/// <summary>
/// Published when a property listing is created by a landlord (in Draft state).
/// Consumed by: Verification (validate landlord identity and property details)
/// </summary>
public sealed record PropertyCreatedIntegrationEvent(
    Guid PropertyId,
    Guid LandlordId,
    string Title,
    string City,
    string Region
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
