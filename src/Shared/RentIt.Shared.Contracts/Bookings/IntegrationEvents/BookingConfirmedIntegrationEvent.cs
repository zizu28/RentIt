using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Bookings.IntegrationEvents;

/// <summary>
/// Published when a booking is confirmed after successful payment.
/// Consumed by: Messaging (notify renter and landlord), Analytics (booking metrics)
/// </summary>
public sealed record BookingConfirmedIntegrationEvent(
    Guid BookingId,
    Guid PropertyId,
    Guid RenterId,
    Guid LandlordId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal TotalAmount,
    string Currency
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
