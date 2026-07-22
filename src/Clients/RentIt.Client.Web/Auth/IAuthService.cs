using RentIt.Shared.DTOs.Identity;

namespace RentIt.Client.Web.Auth;
public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    void InitiateSocialLogin(string provider);
}
