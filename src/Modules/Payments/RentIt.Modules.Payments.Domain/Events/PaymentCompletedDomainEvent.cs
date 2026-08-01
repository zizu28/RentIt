using RentIt.Shared.Abstractions.Domain;

namespace RentIt.Modules.Payments.Domain.Events;

public sealed record PaymentCompletedDomainEvent(
    Guid PaymentId,
    Guid BookingId,
    decimal Amount,
    string Currency,
    string Provider) : DomainEvent;
