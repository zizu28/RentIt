using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Identity.Application.Commands;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result>;
