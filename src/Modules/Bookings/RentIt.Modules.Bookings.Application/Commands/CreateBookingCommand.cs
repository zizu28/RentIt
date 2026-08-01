using MediatR;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Commands;

public record CreateBookingCommand(
    Guid PropertyId,
    Guid GuestId,
    DateOnly StartDate,
    DateOnly EndDate) : IRequest<BookingDto>;
