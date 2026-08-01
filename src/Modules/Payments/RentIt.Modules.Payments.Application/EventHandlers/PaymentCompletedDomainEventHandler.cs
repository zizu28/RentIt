using MediatR;
using RentIt.Modules.Payments.Domain.Events;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;

namespace RentIt.Modules.Payments.Application.EventHandlers;

internal sealed class PaymentCompletedDomainEventHandler(IEventBus eventBus) : INotificationHandler<PaymentCompletedDomainEvent>
{
    private readonly IEventBus _eventBus = eventBus;

    public async Task Handle(PaymentCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Publish integration event to notify cross-module boundaries (e.g. Bookings module)
        // RenterId is not stored in Payment currently, passing Guid.Empty
        var integrationEvent = new PaymentCompletedIntegrationEvent(
            notification.PaymentId,
            notification.BookingId,
            Guid.Empty,
            notification.Amount,
            notification.Currency,
            notification.Provider
        );

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
