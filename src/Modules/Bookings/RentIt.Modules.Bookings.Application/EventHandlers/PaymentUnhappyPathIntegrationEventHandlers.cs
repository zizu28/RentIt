using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Bookings.Application.Services;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;

namespace RentIt.Modules.Bookings.Application.EventHandlers;

internal sealed class PaymentUnhappyPathIntegrationEventHandlers(
    IBookingRepository bookingRepository,
    [FromKeyedServices("Bookings")] IUnitOfWork unitOfWork,
    IBookingsInboxService inboxService) :
    INotificationHandler<PaymentFailedIntegrationEvent>,
    INotificationHandler<PaymentRefundedIntegrationEvent>,
    INotificationHandler<PaymentPartiallyPaidIntegrationEvent>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBookingsInboxService _inboxService = inboxService;

    public async Task Handle(PaymentFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        if (await _inboxService.HasProcessedAsync(notification.EventId, cancellationToken)) return;

        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken)
            ?? throw new BookingDomainException($"Booking {notification.BookingId} not found");

        booking.MarkAsFailed();
        _bookingRepository.Update(booking);
        
        await _inboxService.InsertAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(PaymentRefundedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        if (await _inboxService.HasProcessedAsync(notification.EventId, cancellationToken)) return;

        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken)
            ?? throw new BookingDomainException($"Booking {notification.BookingId} not found");

        booking.MarkAsRefunded();
        _bookingRepository.Update(booking);
        
        await _inboxService.InsertAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(PaymentPartiallyPaidIntegrationEvent notification, CancellationToken cancellationToken)
    {
        if (await _inboxService.HasProcessedAsync(notification.EventId, cancellationToken)) return;

        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken)
            ?? throw new BookingDomainException($"Booking {notification.BookingId} not found");

        booking.MarkAsPartiallyPaid();
        _bookingRepository.Update(booking);
        
        await _inboxService.InsertAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
