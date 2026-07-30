using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.BFF.Middleware;

public class TokenRefreshMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, ILogger<TokenRefreshMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<TokenRefreshMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the user is authenticated
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (authResult.Succeeded && authResult.Properties != null)
            {
                var expiresAtStr = authResult.Properties.GetTokenValue("expires_at");
                var accessToken = authResult.Properties.GetTokenValue("access-token");
                var refreshToken = authResult.Properties.GetTokenValue("refresh-token");

                if (!string.IsNullOrEmpty(expiresAtStr) && DateTimeOffset.TryParse(expiresAtStr, out var expiresAt))
                {
                    // Refresh if expired or expiring within 10 seconds
                    if (expiresAt <= DateTimeOffset.UtcNow.AddSeconds(10))
                    {
                        if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                        {
                            _logger.LogInformation("Access token is expired or expiring soon. Attempting to refresh.");

                            var client = _httpClientFactory.CreateClient("Host");
                            var refreshRequest = new RefreshTokenRequest
                            {
                                AccessToken = accessToken,
                                RefreshToken = refreshToken
                            };

                            try
                            {
                                var response = await client.PostAsJsonAsync("/api/identity/auth/refresh", refreshRequest);

                                if (response.IsSuccessStatusCode)
                                {
                                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                                    if (loginResponse != null)
                                    {
                                        _logger.LogInformation("Token refreshed successfully.");

                                        authResult.Properties.StoreTokens([
                                            new AuthenticationToken { Name = "access-token", Value = loginResponse.AccessToken },
                                            new AuthenticationToken { Name = "refresh-token", Value = loginResponse.RefreshToken },
                                            new AuthenticationToken { Name = "expires_at", Value = DateTimeOffset.UtcNow.AddMinutes(1).ToString("o") }
                                        ]);

                                        // Update the cookie securely
                                        await context.SignInAsync(
                                            CookieAuthenticationDefaults.AuthenticationScheme,
                                            authResult.Principal,
                                            authResult.Properties);
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("Refresh token failed with status code {StatusCode}. Signing out user.", response.StatusCode);
                                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred while refreshing the token.");
                            }
                        }
                    }
                }
            }
        }

        await _next(context);
    }
}
