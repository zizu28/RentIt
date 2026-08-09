using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Shared.Contracts.Identity.Queries;

public sealed record GetUserEmailQuery(Guid UserId) : IRequest<Result<string>>;
