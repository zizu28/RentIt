using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Properties.IntegrationEvents;

/// <summary>
/// Published when a property listing is published and visible to renters.
/// Consumed by: Analytics (index for search and stats)
/// </summary>
public sealed record PropertyPublishedIntegrationEvent(
    Guid PropertyId,
    Guid HostId,
    string Title,
    string City,
    string Region,
    decimal PricePerNight,
    string Currency,
    string ImageUrl,
    int RentalPeriod
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
