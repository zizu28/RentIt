using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.Modules.Identity.Application.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : MediatR.IRequest<Result<LoginResponse>>;
