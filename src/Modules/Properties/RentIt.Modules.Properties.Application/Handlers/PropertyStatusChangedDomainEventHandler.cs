using MediatR;
using RentIt.Modules.Properties.Domain.Enums;
using RentIt.Modules.Properties.Domain.Events;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Properties.IntegrationEvents;

namespace RentIt.Modules.Properties.Application.Handlers;

public sealed class PropertyStatusChangedDomainEventHandler(
    IEventBus eventBus,
    IPropertyRepository propertyRepository,
    Serilog.ILogger logger) : INotificationHandler<PropertyStatusChangedDomainEvent>
{
    private readonly IEventBus _eventBus = eventBus;
    private readonly IPropertyRepository _propertyRepository = propertyRepository;
    private readonly Serilog.ILogger _logger = logger;

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
                property.PricePerPeriod.Currency.ToString(),
                property.Images.FirstOrDefault() ?? string.Empty,
                (int)property.RentalPeriod);

            await _eventBus.PublishAsync(integrationEvent, cancellationToken);

            _logger.Information("Published PropertyPublishedIntegrationEvent for Property {PropertyId}", property.Id);
        }
        else if (notification.OldStatus == PropertyStatus.Available && notification.NewStatus != PropertyStatus.Available)
        {
            _logger.Information("Handling PropertyStatusChangedDomainEvent to {NewStatus} (from Available) for Property {PropertyId}", notification.NewStatus, notification.PropertyId);

            var integrationEvent = new PropertyUnpublishedIntegrationEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                notification.PropertyId);

            await _eventBus.PublishAsync(integrationEvent, cancellationToken);

            _logger.Information("Published PropertyUnpublishedIntegrationEvent for Property {PropertyId}", notification.PropertyId);
        }
    }
}
