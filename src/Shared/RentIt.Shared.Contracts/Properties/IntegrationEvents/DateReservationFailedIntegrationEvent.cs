using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Properties.IntegrationEvents;

/// <summary>
/// Published when date reservation fails (dates unavailable or conflict).
/// Consumed by: Bookings (compensation - mark booking as failed)
/// </summary>
public sealed record DateReservationFailedIntegrationEvent(
    Guid PropertyId,
    Guid BookingId,
    string Reason
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
