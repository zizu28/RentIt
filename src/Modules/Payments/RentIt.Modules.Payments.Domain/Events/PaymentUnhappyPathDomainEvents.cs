using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Payments.Domain.Events;

public sealed record PaymentFailedDomainEvent(
    Guid PaymentId,
    Guid BookingId,
    decimal Amount,
    string Currency,
    string Provider) : DomainEvent;

public sealed record PaymentRefundedDomainEvent(
    Guid PaymentId,
    Guid BookingId,
    decimal AmountRefunded,
    string Currency,
    string Provider) : DomainEvent;

public sealed record PaymentPartiallyPaidDomainEvent(
    Guid PaymentId,
    Guid BookingId,
    decimal AmountPaid,
    decimal TotalAmount,
    string Currency,
    string Provider) : DomainEvent;
