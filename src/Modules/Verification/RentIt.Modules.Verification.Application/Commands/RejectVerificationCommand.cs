using MediatR;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Verification.Application.Commands;

public record RejectVerificationCommand(
    Guid VerificationId,
    string Comments) : IRequest<Result>;
