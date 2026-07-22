using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Properties.IntegrationEvents;

/// <summary>
/// Published when a property's average rating is updated after a new review.
/// Consumed by: Analytics (ranking recalculation)
/// </summary>
public sealed record PropertyRatingUpdatedIntegrationEvent(
    Guid PropertyId,
    double NewAverageRating,
    int TotalReviews
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
