using System.Net.Http.Json;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.Client.Web.Auth;

public class AuthService(
    HttpClient httpClient, 
    BffAuthenticationStateProvider authenticationStateProvider) : IAuthService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly BffAuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;

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
}
