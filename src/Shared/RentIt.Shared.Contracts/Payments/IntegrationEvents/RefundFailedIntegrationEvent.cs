using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Payments.IntegrationEvents;

/// <summary>
/// Published when a refund fails during the cancellation saga.
/// Consumed by: Bookings (trigger retry or manual review escalation)
/// </summary>
public sealed record RefundFailedIntegrationEvent(
    Guid BookingId,
    Guid RenterId,
    decimal AttemptedAmount,
    string Currency,
    string Reason
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
