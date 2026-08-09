using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Bookings.IntegrationEvents;

/// <summary>
/// Published when a booking is cancelled by the renter or Host.
/// Consumed by: Messaging (notify both parties), Analytics (cancellation metrics)
/// </summary>
public sealed record BookingCancelledIntegrationEvent(
    Guid BookingId,
    Guid PropertyId,
    Guid RenterId,
    Guid HostId,
    string CancelledBy,
    string Reason,
    decimal RefundAmount,
    string Currency
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
