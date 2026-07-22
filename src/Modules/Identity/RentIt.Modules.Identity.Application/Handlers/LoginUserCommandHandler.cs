using MediatR;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Modules.Identity.Application.Commands;
using RentIt.Modules.Identity.Domain.Enums;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Contracts.Identity;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginUserCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Get user by email
            var user = await _userRepository.GetByEmailForUpdateAsync(request.Email, cancellationToken);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<LoginResponse>(Error.NotFound(
                    "User.NotFound",
                    "Invalid email or password"));
            }

            // Verify password
            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash.Value))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<LoginResponse>(Error.Validation(
                    "User.InvalidCredentials",
                    "Invalid email or password"));
            }

            // Check if user is active
            if (user.Status != UserStatus.Active)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<LoginResponse>(Error.Validation(
                    "User.NotActive",
                    $"User account is {user.Status.ToString().ToLower()}"));
            }

            // Generate tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email.Value, user.Role.ToString());
            var refreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

            // Add refresh token to user
            var refreshToken = user.AddRefreshToken(refreshTokenString, TimeSpan.FromDays(7));

            // Record login
            user.RecordLogin();

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Map user DTO
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

            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                ExpiresAt = refreshToken.ExpiresAt,
                User = userDto
            };

            return Result.Success(response);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
