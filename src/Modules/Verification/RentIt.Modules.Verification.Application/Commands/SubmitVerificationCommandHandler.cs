using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Verification.Domain.Entities;
using RentIt.Modules.Verification.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Abstractions.Security;

namespace RentIt.Modules.Verification.Application.Commands;

public sealed class SubmitVerificationCommandHandler(
    IHostKycVerificationRepository repository,
    [FromKeyedServices("Verification")] IUnitOfWork unitOfWork,
    IEncryptionService encryptionService) : IRequestHandler<SubmitVerificationCommand, Result<Guid>>
{
    private readonly IHostKycVerificationRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _encryptionService = encryptionService;

    public async Task<Result<Guid>> Handle(SubmitVerificationCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var existing = await _repository.GetByHostIdAsync(request.HostId, cancellationToken);
            if (existing != null && existing.Status != RentIt.Modules.Verification.Domain.Enums.VerificationStatus.Rejected)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<Guid>(Error.Conflict("Verification.Exists", "A verification request is already pending or approved for this host."));
            }

            var encryptedNumber = _encryptionService.Encrypt(request.DocumentNumber);

            var verification = HostKycVerification.RequestVerification(
                request.HostId,
                request.DocumentType,
                encryptedNumber);

            await _repository.AddAsync(verification, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(verification.Id);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
