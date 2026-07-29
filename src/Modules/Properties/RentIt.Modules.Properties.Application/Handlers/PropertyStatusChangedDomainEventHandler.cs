using MediatR;
using Serilog;
using RentIt.Modules.Properties.Domain.Events;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Properties.IntegrationEvents;

namespace RentIt.Modules.Properties.Application.Handlers;

public sealed class PropertyStatusChangedDomainEventHandler : INotificationHandler<PropertyStatusChangedDomainEvent>
{
    private readonly IEventBus _eventBus;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger _logger;

    public PropertyStatusChangedDomainEventHandler(
        IEventBus eventBus, 
        IPropertyRepository propertyRepository,
        ILogger logger)
    {
        _eventBus = eventBus;
        _propertyRepository = propertyRepository;
        _logger = logger;
    }

    public async Task Handle(PropertyStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.NewStatus == PropertyStatus.Available)
        {
            _logger.Information("Handling PropertyStatusChangedDomainEvent to Available for Property {PropertyId}", notification.PropertyId);

            var property = await _propertyRepository.GetByIdAsync(notification.PropertyId, cancellationToken);
            
            if (property is null)
            {
                _logger.Warning("Property {PropertyId} not found when handling PropertyStatusChangedDomainEvent", notification.PropertyId);
                return;
            }

            var integrationEvent = new PropertyPublishedIntegrationEvent(
                property.Id,
                property.HostId,
                property.Name,
                property.Address.City,
                property.Address.Region,
                property.PricePerPeriod.Amount,
                property.PricePerPeriod.Currency.ToString());

            await _eventBus.PublishAsync(integrationEvent, cancellationToken);

            _logger.Information("Published PropertyPublishedIntegrationEvent for Property {PropertyId}", property.Id);
        }
    }
}
