using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Verification.Application.Commands;

public record ApproveVerificationCommand(
    Guid VerificationId,
    string? Comments) : IRequest<Result>;
