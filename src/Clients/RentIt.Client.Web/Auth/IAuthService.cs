using RentIt.Shared.DTOs.Identity;

namespace RentIt.Client.Web.Auth;
public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterUserRequest request);
    Task LogoutAsync();
    Task InitiateSocialLogin(string provider);
}
