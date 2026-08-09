using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Bookings.Application.Commands;

public record CreateBookablePropertyCommand(
    Guid PropertyId,
    string Title,
    string ImageUrl,
    decimal PricePerNight,
    string Currency,
    int RentalPeriod,
    Guid HostId
) : IRequest<Result<Guid>>;
