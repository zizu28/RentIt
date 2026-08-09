using MediatR;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Handlers;

internal sealed class GetHostPendingBookingsQueryHandler(
    IBookingRepository bookingRepository,
    IBookablePropertyRepository propertyRepository) : IRequestHandler<Queries.GetHostPendingBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookablePropertyRepository _propertyRepository = propertyRepository;

    public async Task<IEnumerable<BookingDto>> Handle(Queries.GetHostPendingBookingsQuery request, CancellationToken cancellationToken)
    {
        // For a modular monolith without extensive read-models in this module, 
        // we can fetch properties for the host, then get bookings for those properties.
        // A direct DB query joining the tables would be better, but we will use the repositories.

        // Get all properties for this host
        var hostProperties = await _propertyRepository.GetByHostIdAsync(request.HostId, cancellationToken);

        if (!hostProperties.Any())
        {
            return Enumerable.Empty<BookingDto>();
        }

        var hostPropertyIds = hostProperties.Select(p => p.Id).ToList();

        var pending = await _bookingRepository.GetPendingBookingsByPropertyIdsAsync(hostPropertyIds, cancellationToken);
        
        return pending.Select(b => new BookingDto
        {
            Id = b.Id,
            PropertyId = b.PropertyId,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            TotalPrice = b.TotalPrice.Amount,
            Currency = b.TotalPrice.Currency.ToString(),
            Status = b.Status.ToString()
        }).ToList();
    }
}
