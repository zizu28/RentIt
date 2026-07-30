using MediatR;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Properties;

namespace RentIt.Modules.Properties.Application.Queries;

public sealed record GetPropertyByIdQuery(Guid Id) : IRequest<Result<PropertyDto>>;
