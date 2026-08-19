using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Verification.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Verification.Application.Commands;

public sealed class RejectVerificationCommandHandler(
    IHostKycVerificationRepository repository,
    [FromKeyedServices("Verification")] IUnitOfWork unitOfWork) : IRequestHandler<RejectVerificationCommand, Result>
{
    private readonly IHostKycVerificationRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(RejectVerificationCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var verification = await _repository.GetByIdAsync(request.VerificationId, cancellationToken);
            if (verification == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(Error.NotFound("Verification.NotFound", "The specified verification request was not found."));
            }

            verification.Reject(request.Comments);
            
            _repository.Update(verification);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
