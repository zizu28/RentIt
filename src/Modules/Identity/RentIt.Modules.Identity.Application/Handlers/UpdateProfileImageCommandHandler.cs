using MediatR;
using RentIt.Modules.Identity.Application.Commands;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Abstractions.Storage;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class UpdateProfileImageCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IStorageService storageService) : IRequestHandler<UpdateProfileImageCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IStorageService _storageService = storageService;

    public async Task<Result<UserDto>> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.UserId, out var parsedUserId))
        {
            return Result.Failure<UserDto>(Error.Validation("User.InvalidId", "Invalid user ID format"));
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await _userRepository.GetByIdForUpdateAsync(parsedUserId, cancellationToken);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<UserDto>(Error.NotFound("User.NotFound", "User not found"));
            }

            var imageUrl = await _storageService.UploadImageAsync(request.content, request.filename, cancellationToken);

            user.UpdateProfileImage(imageUrl);
            _userRepository.Update(user, cancellationToken);

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

            return Result.Success(userDto);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
