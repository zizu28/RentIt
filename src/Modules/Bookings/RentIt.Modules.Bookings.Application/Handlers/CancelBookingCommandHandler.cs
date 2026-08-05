using MediatR;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Bookings.Application.Handlers;

public class CancelBookingCommandHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    Serilog.ILogger logger) : IRequestHandler<CancelBookingCommand>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Serilog.ILogger _logger = logger;

    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Attempting to cancel booking {BookingId} for Guest {GuestId}", request.BookingId, request.GuestId);
        
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            _logger.Error("Booking {BookingId} not found for cancellation.", request.BookingId);
            throw new BookingDomainException($"Booking with ID {request.BookingId} not found.");
        }

        if (booking.GuestId != request.GuestId)
        {
            _logger.Warning("Guest {GuestId} is not authorized to cancel booking {BookingId}.", request.GuestId, request.BookingId);
            throw new BookingDomainException("You are not authorized to cancel this booking.");
        }

        booking.Cancel();
        
        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.Information("Successfully cancelled booking {BookingId}", request.BookingId);
    }
}
