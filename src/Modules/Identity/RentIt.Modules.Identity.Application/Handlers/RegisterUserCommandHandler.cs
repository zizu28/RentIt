using MediatR;
using RentIt.Modules.Identity.Domain.Entities;
using RentIt.Modules.Identity.Domain.Enums;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Modules.Identity.Domain.ValueObjects;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Contracts.Identity;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<Commands.RegisterUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<UserDto>> Handle(Commands.RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            return Result.Failure<UserDto>(Error.Conflict(
                "User.EmailExists",
                "A user with this email already exists"));
        }

        if (await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken))
        {
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
            return Result.Failure<UserDto>(Error.Validation(
                "User.InvalidRole",
                "Invalid user role"));
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create user
        var user = User.Create(email, phoneNumber, passwordHash, userRole);

        // Update profile if name provided
        if (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName))
        {
            user.UpdateProfile(request.FirstName, request.LastName);
        }

        // Save user
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);



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
}
