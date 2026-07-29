using RentIt.Shared.DTOs.Identity;

namespace RentIt.Client.Web.Auth;
public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterUserRequest request);
    Task LogoutAsync();
    Task InitiateSocialLogin(string provider);
    Task<bool> VerifyEmailAsync(string email, string token);
    Task<UserDto?> GetCurrentUserAsync();
    Task<bool> UpdateUserProfileAsync(UpdateProfileRequest request);
    Task<UserDto?> UploadProfileImageAsync(Microsoft.AspNetCore.Components.Forms.IBrowserFile file);
}
