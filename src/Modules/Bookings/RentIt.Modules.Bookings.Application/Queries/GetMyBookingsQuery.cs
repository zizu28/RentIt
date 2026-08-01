using MediatR;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Queries;

public record GetMyBookingsQuery(Guid GuestId) : IRequest<IEnumerable<BookingDto>>;
