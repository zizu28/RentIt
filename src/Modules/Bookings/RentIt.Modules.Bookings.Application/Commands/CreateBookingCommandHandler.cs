using MediatR;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Commands;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookablePropertyRepository _propertyRepository;

    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository, 
        IBookablePropertyRepository propertyRepository)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property == null)
        {
            throw new BookingDomainException($"Property with ID {request.PropertyId} not found.");
        }

        var hasOverlapping = await _bookingRepository.HasOverlappingBookingsAsync(request.PropertyId, request.StartDate, request.EndDate, cancellationToken);
        if (hasOverlapping)
        {
            throw new BookingDomainException("The property is already booked for the selected dates.");
        }

        var booking = Booking.Create(
            property.Id,
            request.GuestId,
            request.StartDate,
            request.EndDate,
            property.PricePerNight,
            property.Currency);

        _bookingRepository.Add(booking);
        // Note: unit of work save changes is typically handled via a pipeline behavior or explicitly in the repository

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
