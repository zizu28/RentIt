using MediatR;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Abstractions.Email;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class RequestPasswordResetCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IBackgroundJob backgroundJob) : IRequestHandler<Commands.RequestPasswordResetCommand, Result>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBackgroundJob _backgroundJob = backgroundJob;

    public async Task<Result> Handle(Commands.RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            // Always return success to prevent user enumeration
            return Result.Success();
        }

        var resetToken = Guid.NewGuid().ToString("N");
        user.SetPasswordResetToken(resetToken, TimeSpan.FromMinutes(5));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var resetLink = $"https://localhost:7272/reset-password?token={resetToken}&email={user.Email.Value}";

        _backgroundJob.Enqueue<IEmailService>(
            "default",
            emailService => emailService.SendEmailAsync(
                user.Email.Value,
                "Reset your RentIt Password",
                $"Hi {user.FirstName ?? "there"},\n\nPlease reset your password using the following link: {resetLink}\n\nThis link expires in 5 minutes.",
                CancellationToken.None));

        return Result.Success();
    }
}
