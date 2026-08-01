using RentIt.Shared.Abstractions.Messaging;

namespace RentIt.Shared.Contracts.Payments.IntegrationEvents;

public sealed record PaymentRefundedIntegrationEvent(
    Guid PaymentId,
    Guid BookingId,
    Guid RenterId,
    decimal AmountRefunded,
    string Currency,
    string Provider) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}

public sealed record PaymentPartiallyPaidIntegrationEvent(
    Guid PaymentId,
    Guid BookingId,
    Guid RenterId,
    decimal AmountPaid,
    decimal TotalAmount,
    string Currency,
    string Provider) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
