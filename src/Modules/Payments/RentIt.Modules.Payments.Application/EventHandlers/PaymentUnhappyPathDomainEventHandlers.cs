using MediatR;
using RentIt.Modules.Payments.Domain.Events;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;

namespace RentIt.Modules.Payments.Application.EventHandlers;

internal sealed class PaymentUnhappyPathDomainEventHandlers(IEventBus eventBus) :
    INotificationHandler<PaymentFailedDomainEvent>,
    INotificationHandler<PaymentRefundedDomainEvent>,
    INotificationHandler<PaymentPartiallyPaidDomainEvent>
{
    private readonly IEventBus _eventBus = eventBus;

    public async Task Handle(PaymentFailedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new PaymentFailedIntegrationEvent(
            notification.PaymentId,
            notification.BookingId,
            Guid.Empty,
            "Payment failed",
            "FAILED"
        );
        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }

    public async Task Handle(PaymentRefundedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new PaymentRefundedIntegrationEvent(
            notification.PaymentId,
            notification.BookingId,
            Guid.Empty,
            notification.AmountRefunded,
            notification.Currency,
            notification.Provider
        );
        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }

    public async Task Handle(PaymentPartiallyPaidDomainEvent notification, CancellationToken cancellationToken)
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
        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
