using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Abstractions.Email;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class VerifyEmailCommandHandler(
    IUserRepository userRepository,
    [FromKeyedServices("Identity")] IUnitOfWork unitOfWork,
    IBackgroundJob backgroundJob) : IRequestHandler<Commands.VerifyEmailCommand, Result>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBackgroundJob _backgroundJob = backgroundJob;

    public async Task<Result> Handle(Commands.VerifyEmailCommand request, CancellationToken cancellationToken)
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

            user.VerifyEmail(request.Token);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _backgroundJob.Enqueue<IEmailService>(
                "default",
                emailService => emailService.SendEmailAsync(
                    user.Email.Value,
                    "Email Verified Successfully!",
                    $"Hi {user.FirstName ?? "there"},\n\nYour email has been successfully verified. Thank you!",
                    CancellationToken.None));

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(Error.Validation("User.VerifyEmailFailed", ex.Message));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
