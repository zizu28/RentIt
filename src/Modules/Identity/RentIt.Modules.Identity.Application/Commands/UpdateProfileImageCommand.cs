using MediatR;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.Modules.Identity.Application.Commands;

public sealed record UpdateProfileImageCommand(
    string UserId,
    string ImageUrl
) : IRequest<Result<UserDto>>;
