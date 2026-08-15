using MediatR;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Payments.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;

namespace RentIt.Modules.Bookings.Application.EventHandlers;

public class PaymentInitializationFailedIntegrationEventHandler(
    IBookingRepository bookingRepository,
    [FromKeyedServices("Bookings")] IUnitOfWork unitOfWork,
    Serilog.ILogger logger) : INotificationHandler<PaymentInitializationFailedIntegrationEvent>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Serilog.ILogger _logger = logger;

    public async Task Handle(PaymentInitializationFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information("Received payment initialization failed event for Booking {BookingId}. Reason: {Reason}", notification.BookingId, notification.Reason);

        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);
        
        if (booking == null)
        {
            _logger.Warning("Booking {BookingId} not found when attempting to process failed payment compensation.", notification.BookingId);
            return;
        }

        if (booking.Status == BookingStatus.Pending)
        {
            _logger.Information("Rescinding/Cancelling pending booking {BookingId} due to payment failure.", notification.BookingId);
            booking.Cancel();
            
            _bookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.Information("Successfully rescinded booking {BookingId}.", notification.BookingId);
        }
        else
        {
            _logger.Warning("Booking {BookingId} is in {Status} status, cannot rescind.", notification.BookingId, booking.Status);
        }
    }
}
