using MediatR;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Handlers;

internal sealed class GetHostTransactionsQueryHandler(
    IBookingRepository bookingRepository,
    IBookablePropertyRepository propertyRepository) : IRequestHandler<Queries.GetHostTransactionsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookablePropertyRepository _propertyRepository = propertyRepository;

    public async Task<IEnumerable<BookingDto>> Handle(Queries.GetHostTransactionsQuery request, CancellationToken cancellationToken)
    {
        var hostProperties = await _propertyRepository.GetByHostIdAsync(request.HostId, cancellationToken);

        if (!hostProperties.Any())
        {
            return Enumerable.Empty<BookingDto>();
        }

        var hostPropertyIds = hostProperties.Select(p => p.Id).ToList();

        var bookings = await _bookingRepository.GetBookingsByPropertyIdsAsync(hostPropertyIds, cancellationToken);
        
        return bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            PropertyId = b.PropertyId,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            TotalPrice = b.TotalPrice.Amount,
            Currency = b.TotalPrice.Currency.ToString(),
            Status = b.Status.ToString()
        }).OrderByDescending(b => b.StartDate).ToList();
    }
}
