using MediatR;

namespace RentIt.Modules.Bookings.Application.Commands;

public record CancelBookingCommand(Guid BookingId, Guid GuestId) : IRequest;
