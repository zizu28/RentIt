using MediatR;
using RentIt.Modules.Identity.Domain.Entities;
using RentIt.Modules.Identity.Domain.Enums;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Modules.Identity.Domain.ValueObjects;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Identity;

using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Abstractions.Email;
using Microsoft.Extensions.Configuration;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IBackgroundJob backgroundJob,
    IConfiguration configuration) : IRequestHandler<Commands.RegisterUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBackgroundJob _backgroundJob = backgroundJob;
    private readonly IConfiguration _configuration = configuration;

    public async Task<Result<UserDto>> Handle(Commands.RegisterUserCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Check if user already exists
            if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<UserDto>(Error.Conflict(
                    "User.EmailExists",
                    "A user with this email already exists"));
            }

            if (await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<UserDto>(Error.Conflict(
                    "User.PhoneExists",
                    "A user with this phone number already exists"));
            }

            // Create value objects
            var email = Email.Create(request.Email);
            var phoneNumber = PhoneNumber.Create(request.PhoneNumber);

            // Parse role
            if (!Enum.TryParse<UserRole>(request.Role, out var userRole))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<UserDto>(Error.Validation(
                    "User.InvalidRole",
                    "Invalid user role"));
            }

            // Hash password
            var passwordHashString = _passwordHasher.HashPassword(request.Password);
            var passwordHash = PasswordHash.Create(passwordHashString);

            // Create user
            var user = User.Create(email, phoneNumber, passwordHash, userRole);

            // Update profile if name provided
            if (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName))
            {
                user.UpdateProfile(request.FirstName, request.LastName, null);
            }

            // Generate a random token for verification (stub)
            var verificationToken = Guid.NewGuid().ToString("N");
            user.SetVerificationToken(verificationToken);

            // Save user
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Send Welcome Email and Verification Email in the background
            _backgroundJob.Enqueue<IEmailService>(
                "default", 
                emailService => emailService.SendEmailAsync(
                    user.Email.Value, 
                    "Welcome to RentIt!", 
                    $"Hi {user.FirstName ?? "there"},\n\nWelcome to RentIt! We're glad to have you.", 
                    CancellationToken.None));

            var frontendBaseUrl = _configuration["FrontendBaseUrl"] ?? "https://localhost:7180";
            var verificationLink = $"{frontendBaseUrl}/verify-email?token={verificationToken}&email={user.Email.Value}";

            _backgroundJob.Enqueue<IEmailService>(
                "default", 
                emailService => emailService.SendEmailAsync(
                    user.Email.Value, 
                    "Verify your RentIt Account", 
                    $"Please verify your email by clicking the following link: {verificationLink}", 
                    CancellationToken.None));

            // Map to DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                PhoneNumber = user.PhoneNumber.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                Status = user.Status.ToString(),
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return Result.Success(userDto);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
