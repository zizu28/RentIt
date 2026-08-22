using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Reviews.IntegrationEvents;

/// <summary>
/// Published when a review is submitted and validated for a completed booking.
/// Consumed by: Properties (update average rating), Messaging (notify Host), Analytics (review metrics)
/// </summary>
public sealed record ReviewPublishedIntegrationEvent(
    Guid ReviewId,
    Guid BookingId,
    Guid PropertyId,
    Guid HostId,
    Guid ReviewerId,
    int Rating,
    string? Comment
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
