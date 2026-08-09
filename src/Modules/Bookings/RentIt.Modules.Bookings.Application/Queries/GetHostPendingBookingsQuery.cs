using MediatR;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Queries;

public record GetHostPendingBookingsQuery(Guid HostId) : IRequest<IEnumerable<BookingDto>>;
