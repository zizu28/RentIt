using MediatR;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;

namespace RentIt.Modules.Bookings.Application.EventHandlers;

internal sealed class PaymentUnhappyPathIntegrationEventHandlers(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork) :
    INotificationHandler<PaymentFailedIntegrationEvent>,
    INotificationHandler<PaymentRefundedIntegrationEvent>,
    INotificationHandler<PaymentPartiallyPaidIntegrationEvent>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(PaymentFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);
        if (booking == null)
        {
            return;
        }

        booking.MarkAsFailed();
        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(PaymentRefundedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);
        if (booking == null)
        {
            return;
        }

        booking.MarkAsRefunded();
        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(PaymentPartiallyPaidIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);
        if (booking == null)
        {
            return;
        }

        booking.MarkAsPartiallyPaid();
        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
