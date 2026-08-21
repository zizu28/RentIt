using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Reviews.IntegrationEvents;

namespace RentIt.Modules.Analytics.Application.EventHandlers;

internal sealed class ReviewPublishedIntegrationEventHandler(
    IPropertyMetricsRepository propertyMetricsRepository,
    IHostMetricsRepository hostMetricsRepository,
    [FromKeyedServices("Analytics")] IUnitOfWork unitOfWork) : INotificationHandler<ReviewPublishedIntegrationEvent>
{
    private readonly IPropertyMetricsRepository _propertyMetricsRepository = propertyMetricsRepository;
    private readonly IHostMetricsRepository _hostMetricsRepository = hostMetricsRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(ReviewPublishedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var propertyMetrics = await _propertyMetricsRepository.GetByPropertyIdAsync(notification.PropertyId, cancellationToken);
        
        // If propertyMetrics is null, we can't reliably know the HostId here without querying another module,
        // but it should exist due to PropertyCreatedIntegrationEvent.
        if (propertyMetrics is not null)
        {
            propertyMetrics.AddReview(notification.Rating);
            _propertyMetricsRepository.Update(propertyMetrics);

            // Update Host Metrics
            var hostMetrics = await _hostMetricsRepository.GetByHostIdAsync(propertyMetrics.HostId, cancellationToken);
            if (hostMetrics is null)
            {
                hostMetrics = HostMetrics.Create(propertyMetrics.HostId);
                hostMetrics.AddReview(notification.Rating);
                await _hostMetricsRepository.AddAsync(hostMetrics, cancellationToken);
            }
            else
            {
                hostMetrics.AddReview(notification.Rating);
                _hostMetricsRepository.Update(hostMetrics);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
