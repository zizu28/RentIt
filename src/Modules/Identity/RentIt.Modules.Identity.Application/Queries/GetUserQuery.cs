using MediatR;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.Modules.Identity.Application.Queries;

/// <summary>
/// Query to get user by ID
/// </summary>
public sealed record GetUserQuery(Guid UserId) : IRequest<Result<UserDto>>;
