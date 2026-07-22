using MediatR;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.Modules.Identity.Application.Commands;

/// <summary>
/// Command to login a user via external social provider
/// </summary>
public sealed record SocialLoginCommand(
    string Email,
    string Provider,
    string ProviderKey,
    string? FirstName,
    string? LastName
) : IRequest<Result<LoginResponse>>;
