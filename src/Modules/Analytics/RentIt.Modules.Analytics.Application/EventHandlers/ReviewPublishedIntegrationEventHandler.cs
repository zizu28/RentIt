using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Reviews.IntegrationEvents;

namespace RentIt.Modules.Analytics.Application.EventHandlers;

internal sealed class ReviewPublishedIntegrationEventHandler(
    IPropertyMetricsRepository repository,
    [FromKeyedServices("Analytics")] IUnitOfWork unitOfWork) : INotificationHandler<ReviewPublishedIntegrationEvent>
{
    private readonly IPropertyMetricsRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(ReviewPublishedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var metrics = await _repository.GetByPropertyIdAsync(notification.PropertyId, cancellationToken);
        if (metrics is null)
        {
            metrics = PropertyMetrics.Create(notification.PropertyId);
            metrics.AddReview(notification.Rating);
            await _repository.AddAsync(metrics, cancellationToken);
        }
        else
        {
            metrics.AddReview(notification.Rating);
            _repository.Update(metrics);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
