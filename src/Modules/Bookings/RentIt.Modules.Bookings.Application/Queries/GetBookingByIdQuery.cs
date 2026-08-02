using MediatR;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Queries;

public record GetBookingByIdQuery(Guid BookingId, Guid GuestId) : IRequest<BookingDto>;
