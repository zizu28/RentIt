using RentIt.Modules.Identity.Application.Models;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Identity.Application.Abstractions;

public interface ISocialAuthService
{
    Task<Result<SocialUserProfile>> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}
