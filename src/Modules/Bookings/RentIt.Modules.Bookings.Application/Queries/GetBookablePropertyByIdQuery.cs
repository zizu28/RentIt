using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Modules.Bookings.Application.Queries;

public record GetBookablePropertyByIdQuery(Guid Id) : MediatR.IRequest<Result<BookablePropertyDto>>;
