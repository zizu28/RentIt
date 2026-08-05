using MediatR;
using RentIt.Modules.Bookings.Application.Queries;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Handlers;

public class GetPropertyBookedPeriodsQueryHandler(
    IBookingRepository bookingRepository,
    Serilog.ILogger logger) : IRequestHandler<GetPropertyBookedPeriodsQuery, IReadOnlyList<BookedPeriodDto>>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<IReadOnlyList<BookedPeriodDto>> Handle(GetPropertyBookedPeriodsQuery request, CancellationToken cancellationToken)
    {
        _logger.Information("Fetching booked periods for Property {PropertyId}", request.PropertyId);
        
        var bookings = await _bookingRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        return [.. bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Select(b => new BookedPeriodDto(b.StartDate, b.EndDate))];
    }
}
