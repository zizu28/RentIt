using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Bookings.IntegrationEvents;

/// <summary>
/// Published when a renter requests to book a property.
/// Consumed by: Properties (reserve dates on the calendar)
/// </summary>
public sealed record BookingRequestedIntegrationEvent(
    Guid BookingId,
    Guid PropertyId,
    Guid RenterId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal TotalAmount,
    string Currency
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
