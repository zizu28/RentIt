using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Payments.IntegrationEvents;

/// <summary>
/// Published when a payment is successfully processed for a booking.
/// Consumed by: Bookings (confirm booking in the saga)
/// </summary>
public sealed record PaymentCompletedIntegrationEvent(
    Guid PaymentId,
    Guid BookingId,
    Guid RenterId,
    decimal Amount,
    string Currency,
    string PaymentMethod
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
