using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Identity.Application.Commands;

public sealed record VerifyEmailCommand(string Email, string Token) : IRequest<Result>;
