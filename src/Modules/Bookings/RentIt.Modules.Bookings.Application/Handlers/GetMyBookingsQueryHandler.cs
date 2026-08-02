using MediatR;
using RentIt.Modules.Bookings.Application.Queries;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Handlers;

public class GetMyBookingsQueryHandler(
    IBookingRepository bookingRepository,
    IBookablePropertyRepository propertyRepository) : IRequestHandler<GetMyBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookablePropertyRepository _propertyRepository = propertyRepository;

    public async Task<IEnumerable<BookingDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetByGuestIdAsync(request.GuestId, cancellationToken);
        var dtos = new List<BookingDto>();

        foreach (var booking in bookings)
        {
            var property = await _propertyRepository.GetByIdAsync(booking.PropertyId, cancellationToken);
            if (property == null) continue;

            dtos.Add(new BookingDto
            {
                Id = booking.Id,
                PropertyId = booking.PropertyId,
                PropertyTitle = property.Title,
                PropertyImageUrl = property.ImageUrl,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalPrice = booking.TotalPrice.Amount,
                Status = booking.Status.ToString()
            });
        }

        return dtos.OrderByDescending(b => b.StartDate);
    }
}
