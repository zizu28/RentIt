using MediatR;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Handlers;

public class CreateBookingCommandHandler(
    IBookingRepository bookingRepository, 
    IBookablePropertyRepository propertyRepository,
    IUnitOfWork unitOfWork,
    Serilog.ILogger logger) : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookablePropertyRepository _propertyRepository = propertyRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Attempting to create booking for Property {PropertyId} by Guest {GuestId}", request.PropertyId, request.GuestId);
        
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken) ?? throw new BookingDomainException($"Property with ID {request.PropertyId} not found.");
        var hasOverlapping = await _bookingRepository.HasOverlappingBookingsAsync(request.PropertyId, request.StartDate, request.EndDate, cancellationToken);
        if (hasOverlapping)
        {
            _logger.Warning("Overlapping booking found for Property {PropertyId} between {StartDate} and {EndDate}", request.PropertyId, request.StartDate, request.EndDate);
            throw new BookingDomainException("The property is already booked for the selected dates.");
        }

        var booking = Booking.Create(
            property.Id,
            request.GuestId,
            request.StartDate,
            request.EndDate,
            property.PricePerNight,
            property.Currency,
            property.RentalPeriod);

        _bookingRepository.Add(booking);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.Information("Successfully created booking {BookingId}", booking.Id);

        return new BookingDto
        {
            Id = booking.Id,
            PropertyId = booking.PropertyId,
            PropertyTitle = property.Title,
            PropertyImageUrl = property.ImageUrl,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            TotalPrice = booking.TotalPrice.Amount,
            Status = booking.Status.ToString()
        };
    }
}
