using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Bookings.IntegrationEvents;

namespace RentIt.Modules.Analytics.Application.EventHandlers;

internal sealed class BookingCancelledIntegrationEventHandler(
    IPropertyMetricsRepository propertyMetricsRepository,
    IHostMetricsRepository hostMetricsRepository,
    [FromKeyedServices("Analytics")] IUnitOfWork unitOfWork) : INotificationHandler<BookingCancelledIntegrationEvent>
{
    private readonly IPropertyMetricsRepository _propertyMetricsRepository = propertyMetricsRepository;
    private readonly IHostMetricsRepository _hostMetricsRepository = hostMetricsRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(BookingCancelledIntegrationEvent notification, CancellationToken cancellationToken)
    {
        // Update Property Metrics
        var propertyMetrics = await _propertyMetricsRepository.GetByPropertyIdAsync(notification.PropertyId, cancellationToken);
        if (propertyMetrics is null)
        {
            propertyMetrics = PropertyMetrics.Create(notification.PropertyId, notification.HostId);
            propertyMetrics.IncrementCancellations();
            propertyMetrics.DeductRevenue(notification.RefundAmount); // Deduct refunded amount
            await _propertyMetricsRepository.AddAsync(propertyMetrics, cancellationToken);
        }
        else
        {
            propertyMetrics.IncrementCancellations();
            propertyMetrics.DeductRevenue(notification.RefundAmount); // Deduct refunded amount
            _propertyMetricsRepository.Update(propertyMetrics);
        }

        // Update Host Metrics
        var hostMetrics = await _hostMetricsRepository.GetByHostIdAsync(notification.HostId, cancellationToken);
        if (hostMetrics is null)
        {
            hostMetrics = HostMetrics.Create(notification.HostId);
            hostMetrics.DeductRevenue(notification.RefundAmount); // Deduct refunded amount
            await _hostMetricsRepository.AddAsync(hostMetrics, cancellationToken);
        }
        else
        {
            hostMetrics.DeductRevenue(notification.RefundAmount); // Deduct refunded amount
            _hostMetricsRepository.Update(hostMetrics);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
