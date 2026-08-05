using MediatR;
using RentIt.Modules.Identity.Application.Commands;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Abstractions.Security;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class UpdateUserProfileCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IEncryptionService encryptionService) : IRequestHandler<UpdateUserProfileCommand, Result>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _encryptionService = encryptionService;

    public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(Error.NotFound("User.NotFound", "User not found"));
            }

            var encryptedAddress = string.IsNullOrEmpty(request.Address) ? request.Address : _encryptionService.Encrypt(request.Address);
            user.UpdateProfile(request.FirstName, request.LastName, encryptedAddress, request.Phone);

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
