using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Bookings.Application.Commands;

public record CreateBookablePropertyCommand(
    Guid PropertyId,
    string Title,
    string ImageUrl,
    decimal PricePerNight,
    string Currency
) : MediatR.IRequest<Result<Guid>>;
