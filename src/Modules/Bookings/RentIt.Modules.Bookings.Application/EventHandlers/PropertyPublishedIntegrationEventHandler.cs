using MediatR;
using RentIt.Modules.Bookings.Application.Services;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Properties.IntegrationEvents;

namespace RentIt.Modules.Bookings.Application.EventHandlers;

public class PropertyPublishedIntegrationEventHandler(
    IBookablePropertyRepository propertyRepository,
    IUnitOfWork unitOfWork,
    IBookingsInboxService inboxService,
    Serilog.ILogger logger) : INotificationHandler<PropertyPublishedIntegrationEvent>
{
    private readonly IBookablePropertyRepository _propertyRepository = propertyRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBookingsInboxService _inboxService = inboxService;
    private readonly Serilog.ILogger _logger = logger;

    public async Task Handle(PropertyPublishedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        if (await _inboxService.HasProcessedAsync(@event.EventId, cancellationToken)) return;

        _logger.Information("Handling PropertyPublishedIntegrationEvent for property {PropertyId}", @event.PropertyId);

        var existingProperty = await _propertyRepository.GetByIdAsync(@event.PropertyId, cancellationToken);

        if (existingProperty == null)
        {
            var property = new BookableProperty(
                @event.PropertyId,
                @event.Title,
                @event.ImageUrl, // Use image from the event
                @event.PricePerNight,
                @event.Currency,
                @event.RentalPeriod,
                @event.HostId);

            _propertyRepository.Add(property);
        }
        else
        {
            existingProperty.Update(@event.Title, @event.ImageUrl, @event.PricePerNight, @event.Currency, @event.RentalPeriod, @event.HostId);
            _propertyRepository.Update(existingProperty);
        }

        await _inboxService.InsertAsync(@event, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
