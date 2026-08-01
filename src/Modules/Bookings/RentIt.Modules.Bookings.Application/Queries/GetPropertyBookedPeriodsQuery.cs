using MediatR;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Queries;

public record GetPropertyBookedPeriodsQuery(Guid PropertyId) : IRequest<IReadOnlyList<BookedPeriodDto>>;
