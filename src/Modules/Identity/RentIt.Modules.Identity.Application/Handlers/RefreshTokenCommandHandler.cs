using MediatR;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Modules.Identity.Application.Commands;
using RentIt.Modules.Identity.Domain.Enums;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<LoginResponse>(Error.Unauthorized(
                    "Token.Invalid",
                    "Invalid refresh token"));
            }

            var refreshTokenEntity = user.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);
            if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<LoginResponse>(Error.Unauthorized(
                    "Token.Invalid",
                    "Invalid or expired refresh token"));
            }

            if (user.Status != UserStatus.Active)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<LoginResponse>(Error.Validation(
                    "User.NotActive",
                    $"User account is {user.Status.ToString().ToLower()}"));
            }

            // Revoke old refresh token
            user.RevokeRefreshToken(request.RefreshToken);

            // Generate new tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email.Value, user.Role.ToString());
            var newRefreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

            // Add new refresh token to user
            var newRefreshToken = user.AddRefreshToken(newRefreshTokenString, TimeSpan.FromDays(7));

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

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
                RefreshToken = newRefreshTokenString,
                ExpiresAt = newRefreshToken.ExpiresAt,
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
