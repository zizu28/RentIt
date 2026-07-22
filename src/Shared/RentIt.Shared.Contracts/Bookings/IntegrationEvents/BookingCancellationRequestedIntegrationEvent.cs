using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Bookings.IntegrationEvents;

/// <summary>
/// Published when a booking cancellation is initiated (before refund).
/// Consumed by: Payments (process refund based on cancellation policy)
/// </summary>
public sealed record BookingCancellationRequestedIntegrationEvent(
    Guid BookingId,
    Guid PropertyId,
    Guid RenterId,
    string CancelledBy,
    string Reason,
    decimal OriginalAmount,
    string Currency,
    int DaysUntilCheckIn
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
