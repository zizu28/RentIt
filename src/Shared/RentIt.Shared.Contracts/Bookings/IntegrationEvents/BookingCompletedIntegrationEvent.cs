using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Bookings.IntegrationEvents;

/// <summary>
/// Published when a booking's stay period completes (checkout date passes).
/// Consumed by: Reviews (open review window), Payments (initiate Host payout), Analytics (revenue metrics)
/// </summary>
public sealed record BookingCompletedIntegrationEvent(
    Guid BookingId,
    Guid PropertyId,
    Guid RenterId,
    Guid HostId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal TotalAmount,
    string Currency
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
