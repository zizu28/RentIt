using MediatR;
using RentIt.Modules.Bookings.Domain.Events;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Bookings.IntegrationEvents;

namespace RentIt.Modules.Bookings.Application.EventHandlers;

public sealed class BookingCreatedDomainEventHandler(
    IEventBus eventBus,
    IBookablePropertyRepository propertyRepository,
    Serilog.ILogger logger) : INotificationHandler<BookingCreatedDomainEvent>
{
    private readonly IEventBus _eventBus = eventBus;
    private readonly IBookablePropertyRepository _propertyRepository = propertyRepository;
    private readonly Serilog.ILogger _logger = logger;

    public async Task Handle(BookingCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information("Handling BookingCreatedDomainEvent for Booking {BookingId}", notification.BookingId);

        var property = await _propertyRepository.GetByIdAsync(notification.PropertyId, cancellationToken);
        if (property == null)
        {
            _logger.Warning("Bookable property {PropertyId} not found. Cannot publish BookingRequestedIntegrationEvent.", notification.PropertyId);
            return;
        }

        var integrationEvent = new BookingRequestedIntegrationEvent(
            notification.BookingId,
            notification.PropertyId,
            property.HostId,
            notification.GuestId,
            notification.StartDate,
            notification.EndDate,
            notification.TotalPrice,
            notification.Currency);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        _logger.Information("Published BookingRequestedIntegrationEvent for Booking {BookingId}", notification.BookingId);
    }
}
