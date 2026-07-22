using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Payments.IntegrationEvents;

/// <summary>
/// Published when a refund is successfully processed for a cancelled booking.
/// Consumed by: Bookings (proceed with date release in cancellation saga)
/// </summary>
public sealed record RefundProcessedIntegrationEvent(
    Guid RefundId,
    Guid BookingId,
    Guid RenterId,
    decimal RefundAmount,
    decimal OriginalAmount,
    string Currency
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
