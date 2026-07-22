using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Reviews.IntegrationEvents;

/// <summary>
/// Published when a review is removed (moderation, user deletion, etc.).
/// Consumed by: Properties (recalculate average rating without the removed review)
/// </summary>
public sealed record ReviewRemovedIntegrationEvent(
    Guid ReviewId,
    Guid PropertyId,
    int RemovedRating
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
