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

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    [FromKeyedServices("Identity")] IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var oldToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (oldToken == null || !oldToken.IsActive)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<LoginResponse>(Error.Unauthorized("Token.Invalid", "Refresh token is invalid or expired."));
            }

            var user = await _userRepository.GetByIdAsync(oldToken.UserId, cancellationToken);
            if (user == null || user.Status != UserStatus.Active)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<LoginResponse>(Error.Unauthorized("User.Invalid", "User is invalid or inactive."));
            }

            // Revoke old token
            oldToken.Revoke();
            _refreshTokenRepository.Update(oldToken);

            // Generate new tokens
            var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email.Value, user.Role.ToString());
            var newRefreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

            var newRefreshToken = RentIt.Modules.Identity.Domain.Entities.RefreshToken.Create(user.Id, newRefreshTokenString, TimeSpan.FromDays(7));
            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

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

            return Result.Success(new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenString,
                ExpiresAt = newRefreshToken.ExpiresAt,
                User = userDto
            });
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
