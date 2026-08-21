using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Properties.IntegrationEvents;

namespace RentIt.Modules.Analytics.Application.EventHandlers;

internal sealed class PropertyCreatedIntegrationEventHandler(
    IPropertyMetricsRepository propertyMetricsRepository,
    IHostMetricsRepository hostMetricsRepository,
    [FromKeyedServices("Analytics")] IUnitOfWork unitOfWork) : INotificationHandler<PropertyCreatedIntegrationEvent>
{
    private readonly IPropertyMetricsRepository _propertyMetricsRepository = propertyMetricsRepository;
    private readonly IHostMetricsRepository _hostMetricsRepository = hostMetricsRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(PropertyCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        // Initialize Property Metrics
        var propertyMetrics = await _propertyMetricsRepository.GetByPropertyIdAsync(notification.PropertyId, cancellationToken);
        if (propertyMetrics is null)
        {
            propertyMetrics = PropertyMetrics.Create(notification.PropertyId, notification.HostId);
            await _propertyMetricsRepository.AddAsync(propertyMetrics, cancellationToken);
        }

        // Update Host Metrics
        var hostMetrics = await _hostMetricsRepository.GetByHostIdAsync(notification.HostId, cancellationToken);
        if (hostMetrics is null)
        {
            hostMetrics = HostMetrics.Create(notification.HostId);
            hostMetrics.IncrementProperties();
            await _hostMetricsRepository.AddAsync(hostMetrics, cancellationToken);
        }
        else
        {
            hostMetrics.IncrementProperties();
            _hostMetricsRepository.Update(hostMetrics);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
