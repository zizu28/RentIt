using MediatR;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Bookings.Application.Commands;

public record RescindBookingCommand(Guid BookingId, Guid HostId) : IRequest;

public class RescindBookingCommandHandler(
    IBookingRepository bookingRepository,
    IBookablePropertyRepository propertyRepository,
    IUnitOfWork unitOfWork,
    Serilog.ILogger logger) : IRequestHandler<RescindBookingCommand>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookablePropertyRepository _propertyRepository = propertyRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Serilog.ILogger _logger = logger;

    public async Task Handle(RescindBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Attempting to rescind booking {BookingId} by Host {HostId}", request.BookingId, request.HostId);
        
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            _logger.Error("Booking {BookingId} not found for cancellation.", request.BookingId);
            throw new BookingDomainException($"Booking with ID {request.BookingId} not found.");
        }

        var property = await _propertyRepository.GetByIdAsync(booking.PropertyId, cancellationToken);
        if (property == null || property.HostId != request.HostId)
        {
            _logger.Warning("Host {HostId} is not authorized to rescind booking {BookingId}.", request.HostId, request.BookingId);
            throw new BookingDomainException("You are not authorized to cancel this booking.");
        }

        booking.Cancel();
        
        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.Information("Successfully rescinded booking {BookingId}", request.BookingId);
    }
}
