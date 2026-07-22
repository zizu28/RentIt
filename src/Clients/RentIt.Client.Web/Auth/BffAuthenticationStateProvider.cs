using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace RentIt.Client.Web.Auth;

public class BffAuthenticationStateProvider(HttpClient httpClient) : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient = httpClient;
    private static readonly TimeSpan UserCacheDuration = TimeSpan.FromMinutes(1);
    private DateTimeOffset _userLastCheck = DateTimeOffset.MinValue;
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new AuthenticationState(await GetUserAsync());
    }

    private async Task<ClaimsPrincipal> GetUserAsync(bool useCache = true)
    {
        var now = DateTimeOffset.Now;
        if (useCache && now < _userLastCheck + UserCacheDuration)
        {
            return _cachedUser;
        }

        try
        {
            var response = await _httpClient.GetAsync("/bff/auth/user");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserInfo>();
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, user.Id),
                        new(ClaimTypes.Email, user.Email),
                        new(ClaimTypes.Role, user.Role)
                    };

                    _cachedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "BFF"));
                }
            }
            else
            {
                _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
        catch
        {
            _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        _userLastCheck = now;
        return _cachedUser;
    }

    public void NotifyUserAuthenticationStateChanged()
    {
        // Force refresh of the cache
        _userLastCheck = DateTimeOffset.MinValue;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
