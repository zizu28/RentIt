using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.BFF.Middleware;

public sealed class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TokenRefreshMiddleware> _logger;

    public TokenRefreshMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, ILogger<TokenRefreshMiddleware> logger)
    {
        _next = next;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Don't intercept auth-related calls or static files
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/bff/auth") || path.StartsWith("/api/identity/auth"))
        {
            await _next(context);
            return;
        }

        var authenticateResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (authenticateResult.Succeeded && authenticateResult.Principal != null)
        {
            var expiresAtToken = authenticateResult.Properties.GetTokenValue("expires_at");
            
            if (DateTimeOffset.TryParse(expiresAtToken, out var expiresAt))
            {
                // Refresh if token expires in less than 30 seconds
                if (expiresAt.UtcDateTime < DateTime.UtcNow.AddSeconds(30))
                {
                    var refreshToken = authenticateResult.Properties.GetTokenValue("refresh-token");
                    
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        try
                        {
                            var client = _httpClientFactory.CreateClient("Host");
                            var request = new RefreshTokenRequest { RefreshToken = refreshToken };
                            
                            var response = await client.PostAsJsonAsync("/api/identity/auth/refresh", request);
                            
                            if (response.IsSuccessStatusCode)
                            {
                                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                                
                                if (loginResponse != null)
                                {
                                    authenticateResult.Properties.UpdateTokenValue("access-token", loginResponse.AccessToken);
                                    authenticateResult.Properties.UpdateTokenValue("refresh-token", loginResponse.RefreshToken);
                                    authenticateResult.Properties.UpdateTokenValue("expires_at", loginResponse.ExpiresAt.ToString("o"));
                                    
                                    await context.SignInAsync(
                                        CookieAuthenticationDefaults.AuthenticationScheme,
                                        authenticateResult.Principal,
                                        authenticateResult.Properties);
                                        
                                    _logger.LogInformation("Successfully refreshed access token for user {UserId}", 
                                        authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier));
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Failed to refresh token. Status: {StatusCode}", response.StatusCode);
                                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error refreshing token");
                        }
                    }
                }
            }
        }

        await _next(context);
    }
}
