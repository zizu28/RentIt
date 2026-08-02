using MediatR;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Properties.IntegrationEvents;

namespace RentIt.Modules.Bookings.Application.EventHandlers;

public sealed class PropertyUnpublishedIntegrationEventHandler(
    IBookablePropertyRepository propertyRepository,
    IUnitOfWork unitOfWork,
    Serilog.ILogger logger
) : INotificationHandler<PropertyUnpublishedIntegrationEvent>
{
    private readonly IBookablePropertyRepository _propertyRepository = propertyRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Serilog.ILogger _logger = logger;

    public async Task Handle(PropertyUnpublishedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.Information("Handling PropertyUnpublishedIntegrationEvent for Property {PropertyId}", @event.PropertyId);

        var property = await _propertyRepository.GetByIdAsync(@event.PropertyId, cancellationToken);
        if (property is null)
        {
            _logger.Information("Property {PropertyId} was already removed or never added to Bookings.", @event.PropertyId);
            return;
        }

        _propertyRepository.Remove(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.Information("Successfully removed BookableProperty {PropertyId} due to unpublish event.", @event.PropertyId);
    }
}
