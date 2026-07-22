using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Properties.IntegrationEvents;

/// <summary>
/// Published when reserved dates are released (e.g. booking cancelled or reservation failed).
/// Consumed by: Bookings (confirm dates are freed)
/// </summary>
public sealed record DatesReleasedIntegrationEvent(
    Guid PropertyId,
    Guid BookingId,
    DateOnly StartDate,
    DateOnly EndDate
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
