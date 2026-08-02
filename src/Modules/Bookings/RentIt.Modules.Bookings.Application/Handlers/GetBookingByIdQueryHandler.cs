using MediatR;
using RentIt.Modules.Bookings.Application.Queries;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Handlers;

public class GetBookingByIdQueryHandler(
    IBookingRepository bookingRepository,
    IBookablePropertyRepository propertyRepository) : IRequestHandler<GetBookingByIdQuery, BookingDto>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookablePropertyRepository _propertyRepository = propertyRepository;

    public async Task<BookingDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            throw new BookingDomainException($"Booking with ID {request.BookingId} not found.");
        }

        if (booking.GuestId != request.GuestId)
        {
            throw new BookingDomainException("You are not authorized to view this booking.");
        }

        var property = await _propertyRepository.GetByIdAsync(booking.PropertyId, cancellationToken);
        var propertyTitle = property?.Title ?? "Unknown Property";
        var propertyImageUrl = property?.ImageUrl ?? string.Empty;

        return new BookingDto
        {
            Id = booking.Id,
            PropertyId = booking.PropertyId,
            PropertyTitle = propertyTitle,
            PropertyImageUrl = propertyImageUrl,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            TotalPrice = booking.TotalPrice.Amount,
            Status = booking.Status.ToString()
        };
    }
}
