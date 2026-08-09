using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Modules.Identity.Application.Commands;
using RentIt.Modules.Identity.Domain.Enums;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    [FromKeyedServices("Identity")] IUnitOfWork unitOfWork) : IRequestHandler<LoginUserCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
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
                return Result.Failure<LoginResponse>(Error.Unauthorized(
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
            var refreshTokenString = string.Empty; //_jwtTokenGenerator.GenerateRefreshToken();

            //var refreshToken = RentIt.Modules.Identity.Domain.Entities.RefreshToken.Create(user.Id, refreshTokenString, TimeSpan.FromDays(7));
            //await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

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
                LastLoginAt = user.LastLoginAt,
                ProfileImageUrl = user.ProfileImageUrl
            };

            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30), //refreshToken.ExpiresAt,
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
