using MediatR;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;

namespace RentIt.Modules.Bookings.Application.EventHandlers;

internal sealed class PaymentCompletedIntegrationEventHandler(IBookingRepository bookingRepository) 
    : INotificationHandler<PaymentCompletedIntegrationEvent>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;

    public async Task Handle(PaymentCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);
        
        if (booking == null)
            return;

        if (booking.Status == Domain.Enums.BookingStatus.Pending)
        {
            booking.Confirm();
            _bookingRepository.Update(booking);
        }
    }
}
