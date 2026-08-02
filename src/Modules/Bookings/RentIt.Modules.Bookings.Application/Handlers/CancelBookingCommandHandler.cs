using MediatR;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Bookings.Application.Handlers;

public class CancelBookingCommandHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelBookingCommand>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            throw new BookingDomainException($"Booking with ID {request.BookingId} not found.");
        }

        if (booking.GuestId != request.GuestId)
        {
            throw new BookingDomainException("You are not authorized to cancel this booking.");
        }

        booking.Cancel();
        
        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
