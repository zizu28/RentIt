using MediatR;
using RentIt.Modules.Bookings.Application.Queries;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Handlers;

public class GetPropertyBookedPeriodsQueryHandler(IBookingRepository bookingRepository) : IRequestHandler<GetPropertyBookedPeriodsQuery, IReadOnlyList<BookedPeriodDto>>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;

    public async Task<IReadOnlyList<BookedPeriodDto>> Handle(GetPropertyBookedPeriodsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        return [.. bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Select(b => new BookedPeriodDto(b.StartDate, b.EndDate))];
    }
}
