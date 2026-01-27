using MediatR;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Contracts.Identity;

namespace RentIt.Modules.Identity.Application.Commands;

/// <summary>
/// Command to login a user
/// </summary>
public sealed record LoginUserCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>;
