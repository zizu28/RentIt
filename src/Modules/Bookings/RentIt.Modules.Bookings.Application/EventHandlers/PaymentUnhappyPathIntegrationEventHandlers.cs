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
        if (booking == null) return;

        // We leave the booking in Pending state on failure, but log it.
        // A full implementation might track retry attempts or cancel it if max retries exceeded.
    }

    public async Task Handle(PaymentRefundedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);
        if (booking == null) return;

        // Using reflection or a dedicated method in Booking aggregate to set status
        // Since we don't have MarkAsRefunded in Booking yet, we will just update status manually
        // In true DDD, Booking should have a MarkAsRefunded() method.
        booking.Cancel(); // Simplest approach for now: cancel the booking to free dates

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(PaymentPartiallyPaidIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);
        if (booking == null) return;

        // Leave it pending, wait for full payment. Or add a MarkAsPartiallyPaid if business needs it.
    }
}
