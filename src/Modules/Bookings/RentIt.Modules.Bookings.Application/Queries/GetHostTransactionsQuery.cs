using MediatR;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Queries;

public record GetHostTransactionsQuery(Guid HostId) : IRequest<IEnumerable<BookingDto>>;
