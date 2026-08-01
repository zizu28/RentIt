using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Contracts.Properties.IntegrationEvents;
using RentIt.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace RentIt.Modules.Bookings.Application.EventHandlers;

public class PropertyPublishedIntegrationEventHandler : IIntegrationEventHandler<PropertyPublishedIntegrationEvent>
{
    private readonly IBookablePropertyRepository _propertyRepository;
    private readonly ILogger<PropertyPublishedIntegrationEventHandler> _logger;

    public PropertyPublishedIntegrationEventHandler(
        IBookablePropertyRepository propertyRepository,
        ILogger<PropertyPublishedIntegrationEventHandler> logger)
    {
        _propertyRepository = propertyRepository;
        _logger = logger;
    }

    public async Task HandleAsync(PropertyPublishedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling PropertyPublishedIntegrationEvent for property {PropertyId}", @event.PropertyId);

        var existingProperty = await _propertyRepository.GetByIdAsync(@event.PropertyId, cancellationToken);
        
        if (existingProperty == null)
        {
            var property = new BookableProperty(
                @event.PropertyId,
                @event.Title,
                "", // Default image url
                @event.PricePerNight,
                @event.Currency);
            
            _propertyRepository.Add(property);
        }
        else
        {
            existingProperty.Update(@event.Title, @event.PricePerNight, @event.Currency);
            _propertyRepository.Update(existingProperty);
        }
    }
}
