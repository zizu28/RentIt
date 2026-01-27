using MediatR;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Contracts.Identity;

namespace RentIt.Modules.Identity.Application.Commands;

/// <summary>
/// Command to register a new user
/// </summary>
public sealed record RegisterUserCommand(
    string Email,
    string PhoneNumber,
    string Password,
    string Role,
    string? FirstName,
    string? LastName
) : IRequest<Result<UserDto>>;
