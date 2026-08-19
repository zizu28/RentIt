using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Bookings.IntegrationEvents;

namespace RentIt.Modules.Analytics.Application.EventHandlers;

internal sealed class BookingConfirmedIntegrationEventHandler(
    IPropertyMetricsRepository repository,
    [FromKeyedServices("Analytics")] IUnitOfWork unitOfWork) : INotificationHandler<BookingConfirmedIntegrationEvent>
{
    private readonly IPropertyMetricsRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(BookingConfirmedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var metrics = await _repository.GetByPropertyIdAsync(notification.PropertyId, cancellationToken);
        if (metrics is null)
        {
            metrics = PropertyMetrics.Create(notification.PropertyId);
            metrics.IncrementBookings();
            await _repository.AddAsync(metrics, cancellationToken);
        }
        else
        {
            metrics.IncrementBookings();
            _repository.Update(metrics);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
