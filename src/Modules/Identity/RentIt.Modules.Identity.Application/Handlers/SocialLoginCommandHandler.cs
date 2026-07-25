using MediatR;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Modules.Identity.Application.Commands;
using RentIt.Modules.Identity.Domain.Entities;
using RentIt.Modules.Identity.Domain.Enums;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Modules.Identity.Domain.ValueObjects;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class SocialLoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork,
    ISocialAuthServiceFactory socialAuthServiceFactory) : IRequestHandler<SocialLoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ISocialAuthServiceFactory _socialAuthServiceFactory = socialAuthServiceFactory;

    public async Task<Result<LoginResponse>> Handle(SocialLoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Get the correct auth service for the provider
        var authService = _socialAuthServiceFactory.Create(request.Provider);
        
        // 2. Validate the token and get the user profile
        var profileResult = await authService.ValidateTokenAsync(request.AccessToken, cancellationToken);
        if (profileResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(profileResult.Error);
        }
        
        var profile = profileResult.Value;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Get user by email
            var user = await _userRepository.GetByEmailForUpdateAsync(profile.Email, cancellationToken);
            
            if (user == null)
            {
                // Create user if they don't exist
                var randomPassword = Guid.NewGuid().ToString("N") + "Aa1!";
                var hash = _passwordHasher.HashPassword(randomPassword);
                var passwordHash = PasswordHash.Create(hash);
                var email = Email.Create(profile.Email);
                var phoneNumber = PhoneNumber.Create("0000000000"); // Placeholder
                
                user = User.Create(email, phoneNumber, passwordHash, UserRole.Renter);
                user.UpdateProfile(profile.FirstName, profile.LastName);
                // Mark email as verified since it came from a trusted provider
                user.SetVerificationToken("SOCIAL_LOGIN");
                user.VerifyEmail("SOCIAL_LOGIN");
                
                await _userRepository.AddAsync(user, cancellationToken);
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
