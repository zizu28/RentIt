using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Identity.Application.Commands;

public sealed record UpdateUserProfileCommand(Guid UserId, string? FirstName, string? LastName, string? Address) : IRequest<Result>;
