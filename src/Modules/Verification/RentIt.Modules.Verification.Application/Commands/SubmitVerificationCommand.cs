using MediatR;
using RentIt.Modules.Verification.Domain.Enums;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Verification.Application.Commands;

public record SubmitVerificationCommand(
    Guid HostId,
    DocumentType DocumentType,
    string DocumentNumber) : IRequest<Result<Guid>>;
