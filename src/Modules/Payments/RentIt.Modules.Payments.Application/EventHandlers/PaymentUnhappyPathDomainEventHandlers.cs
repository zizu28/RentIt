using MediatR;
using RentIt.Modules.Payments.Application.Services;
using RentIt.Modules.Payments.Domain.Events;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;

namespace RentIt.Modules.Payments.Application.EventHandlers;

internal sealed class PaymentUnhappyPathDomainEventHandlers(IPaymentsOutboxService outboxService) :
    INotificationHandler<PaymentFailedDomainEvent>,
    INotificationHandler<PaymentRefundedDomainEvent>,
    INotificationHandler<PaymentPartiallyPaidDomainEvent>
{
    private readonly IPaymentsOutboxService _outboxService = outboxService;

    public Task Handle(PaymentFailedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new PaymentFailedIntegrationEvent(
            notification.PaymentId,
            notification.BookingId,
            Guid.Empty,
            "Payment failed",
            "FAILED"
        );
        _outboxService.Add(integrationEvent);
        return Task.CompletedTask;
    }

    public Task Handle(PaymentRefundedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new PaymentRefundedIntegrationEvent(
            notification.PaymentId,
            notification.BookingId,
            Guid.Empty,
            notification.AmountRefunded,
            notification.Currency,
            notification.Provider
        );
        _outboxService.Add(integrationEvent);
        return Task.CompletedTask;
    }

    public Task Handle(PaymentPartiallyPaidDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new PaymentPartiallyPaidIntegrationEvent(
            notification.PaymentId,
            notification.BookingId,
            Guid.Empty,
            notification.AmountPaid,
            notification.TotalAmount,
            notification.Currency,
            notification.Provider
        );
        _outboxService.Add(integrationEvent);
        return Task.CompletedTask;
    }
}
