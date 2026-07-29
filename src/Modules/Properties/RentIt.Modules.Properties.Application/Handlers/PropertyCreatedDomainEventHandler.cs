using MediatR;
using Serilog;
using RentIt.Modules.Properties.Domain.Events;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Properties.IntegrationEvents;

namespace RentIt.Modules.Properties.Application.Handlers;

public sealed class PropertyCreatedDomainEventHandler : INotificationHandler<PropertyCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger _logger;

    public PropertyCreatedDomainEventHandler(
        IEventBus eventBus, 
        IPropertyRepository propertyRepository,
        ILogger logger)
    {
        _eventBus = eventBus;
        _propertyRepository = propertyRepository;
        _logger = logger;
    }

    public async Task Handle(PropertyCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information("Handling PropertyCreatedDomainEvent for Property {PropertyId}", notification.PropertyId);

        var property = await _propertyRepository.GetByIdAsync(notification.PropertyId, cancellationToken);
        
        if (property is null)
        {
            _logger.Warning("Property {PropertyId} not found when handling PropertyCreatedDomainEvent", notification.PropertyId);
            return;
        }

        var integrationEvent = new PropertyCreatedIntegrationEvent(
            property.Id,
            property.HostId,
            property.Name,
            property.Address.City,
            property.Address.Region);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        _logger.Information("Published PropertyCreatedIntegrationEvent for Property {PropertyId}", property.Id);
    }
}
