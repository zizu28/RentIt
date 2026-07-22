using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Payments.IntegrationEvents;

/// <summary>
/// Published when a payment fails during the booking saga.
/// Consumed by: Bookings (compensation - release dates and cancel booking)
/// </summary>
public sealed record PaymentFailedIntegrationEvent(
    Guid PaymentId,
    Guid BookingId,
    Guid RenterId,
    string Reason,
    string FailureCode
) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
