using MediatR;
using RentIt.Modules.Identity.Domain.Repositories;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Contracts.Identity.Queries;

namespace RentIt.Modules.Identity.Application.Queries;

internal sealed class GetUserEmailQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserEmailQuery, Result<string>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<string>> Handle(GetUserEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<string>(Error.NotFound("User.NotFound", "The user was not found"));
        }

        return Result.Success(user.Email.Value);
    }
}
