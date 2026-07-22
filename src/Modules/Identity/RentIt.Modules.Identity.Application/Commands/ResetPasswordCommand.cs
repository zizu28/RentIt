using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Identity.Application.Commands;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result>;
