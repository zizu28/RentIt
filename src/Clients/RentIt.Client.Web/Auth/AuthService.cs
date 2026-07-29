using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.Client.Web.Auth;

public class AuthService(
    HttpClient httpClient, 
    BffAuthenticationStateProvider authenticationStateProvider,
    NavigationManager navigationManager) : IAuthService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly BffAuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;
    private readonly NavigationManager _navigationManager = navigationManager;

    public async Task InitiateSocialLogin(string provider)
    {
        var response = await _httpClient.GetAsync($"/bff/auth/challenge/{provider}");
        
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<SocialAuthUrlResponse>();
            if (data?.Url != null)
            {
                _navigationManager.NavigateTo(data.Url, forceLoad: true);
            }
        }
    }

    private class SocialAuthUrlResponse
    {
        public string Url { get; set; } = string.Empty;
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/bff/auth/login", request);
        
        if (response.IsSuccessStatusCode)
        {
            _authenticationStateProvider.NotifyUserAuthenticationStateChanged();
            return true;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await _httpClient.PostAsync("/bff/auth/logout", null);
        _authenticationStateProvider.NotifyUserAuthenticationStateChanged();
    }

    public async Task<bool> RegisterAsync(RegisterUserRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/identity/auth/register", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> VerifyEmailAsync(string email, string token)
    {
        try
        {
            var request = new VerifyEmailRequest { Email = email, Token = token };
            var response = await _httpClient.PostAsJsonAsync("/api/identity/auth/verify-email", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserDto>("/api/identity/users/me");
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> UpdateUserProfileAsync(UpdateProfileRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("/api/identity/users/me/profile", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<UserDto?> UploadProfileImageAsync(Microsoft.AspNetCore.Components.Forms.IBrowserFile file)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            // Maximum of 5MB
            var stream = file.OpenReadStream(5 * 1024 * 1024);
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync("/api/identity/users/me/profile-image", content);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
