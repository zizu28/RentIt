using MediatR;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Queries;

public class GetPropertyBookedPeriodsQueryHandler : IRequestHandler<GetPropertyBookedPeriodsQuery, IReadOnlyList<BookedPeriodDto>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetPropertyBookedPeriodsQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IReadOnlyList<BookedPeriodDto>> Handle(GetPropertyBookedPeriodsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _bookingRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);
        
        return bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Select(b => new BookedPeriodDto(b.StartDate, b.EndDate))
            .ToList();
    }
}
