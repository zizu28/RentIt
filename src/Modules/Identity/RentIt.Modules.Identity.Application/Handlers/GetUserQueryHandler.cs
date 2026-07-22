using MediatR;
using RentIt.Modules.Identity.Application.Queries;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Contracts.Identity;

namespace RentIt.Modules.Identity.Application.Handlers;

public sealed class GetUserQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<UserDto>(Error.NotFound(
                "User.NotFound",
                "User not found"));
        }

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
