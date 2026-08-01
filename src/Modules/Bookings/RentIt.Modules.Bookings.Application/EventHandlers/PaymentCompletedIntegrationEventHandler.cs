using MediatR;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;

namespace RentIt.Modules.Bookings.Application.EventHandlers;

internal sealed class PaymentCompletedIntegrationEventHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork) : INotificationHandler<PaymentCompletedIntegrationEvent>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(PaymentCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);

        if (booking == null)
            return;

        if (booking.Status == BookingStatus.Pending)
        {
            booking.Confirm();
            _bookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
