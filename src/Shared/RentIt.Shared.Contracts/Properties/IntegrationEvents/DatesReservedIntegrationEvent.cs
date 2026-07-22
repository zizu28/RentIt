using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Properties.IntegrationEvents;

/// <summary>
/// Published when property dates are successfully reserved for a booking.
/// Consumed by: Bookings (proceed to payment step in the saga)
/// </summary>
public sealed record DatesReservedIntegrationEvent(
    Guid PropertyId,
    Guid BookingId,
    DateOnly StartDate,
    DateOnly EndDate
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
