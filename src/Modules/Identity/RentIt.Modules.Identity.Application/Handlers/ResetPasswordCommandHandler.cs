using MediatR;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Modules.Identity.Domain.ValueObjects;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Abstractions.Email;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IBackgroundJob backgroundJob) : IRequestHandler<Commands.ResetPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBackgroundJob _backgroundJob = backgroundJob;

    public async Task<Result> Handle(Commands.ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await _userRepository.GetByEmailForUpdateAsync(request.Email, cancellationToken);

            if (user is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(Error.NotFound("User.NotFound", "User not found"));
            }

            var passwordHashString = _passwordHasher.HashPassword(request.NewPassword);
            var passwordHash = PasswordHash.Create(passwordHashString);

            user.ResetPassword(request.Token, passwordHash);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _backgroundJob.Enqueue<IEmailService>(
                "default",
                emailService => emailService.SendEmailAsync(
                    user.Email.Value,
                    "Password Reset Successfully",
                    $"Hi {user.FirstName ?? "there"},\n\nYour password has been successfully reset. If you did not do this, please contact support immediately.",
                    CancellationToken.None));

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(Error.Validation("User.ResetPasswordFailed", ex.Message));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
