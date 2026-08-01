using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.BFF.Controllers;

[ApiController]
[Route("bff/auth")]
public sealed class AuthController(IHttpClientFactory httpClientFactory, ILogger<AuthController> logger) : ControllerBase
{
#pragma warning disable
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("Host");

        var response = await client.PostAsJsonAsync("/api/identity/auth/login", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            return BadRequest(new { error = "Invalid login attempt." });
        }

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);

        if (loginResponse == null)
            return BadRequest(new { error = "Invalid login response." });

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, loginResponse.User.Id.ToString()),
            new Claim(ClaimTypes.Email, loginResponse.User.Email),
            new Claim(ClaimTypes.Role, loginResponse.User.Role)
        };

        if (!string.IsNullOrEmpty(loginResponse.User.FirstName))
            claims.Add(new Claim(ClaimTypes.GivenName, loginResponse.User.FirstName));
            
        if (!string.IsNullOrEmpty(loginResponse.User.LastName))
            claims.Add(new Claim(ClaimTypes.Surname, loginResponse.User.LastName));
            
        if (!string.IsNullOrEmpty(loginResponse.User.ProfileImageUrl))
            claims.Add(new Claim("ProfileImageUrl", loginResponse.User.ProfileImageUrl));

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        authProperties.StoreTokens([
            new AuthenticationToken { Name = "access-token", Value = loginResponse.AccessToken },
            new AuthenticationToken { Name = "refresh-token", Value = loginResponse.RefreshToken },
            new AuthenticationToken { Name = "expires_at", Value = loginResponse.ExpiresAt.ToString("o") }
        ]);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return Ok(new { message = "Logged in successfully" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("challenge/{provider}")]
    public IActionResult Challenge(string provider)
    {
        var redirectUrl = Url.Action(nameof(Callback));
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        properties.Items["provider"] = provider;
        return Challenge(properties, provider);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(CancellationToken cancellationToken)
    {
        var result = await HttpContext.AuthenticateAsync("ExternalCookie");
        if (!result.Succeeded || result.Principal == null)
            return Redirect("https://localhost:7180/login?error=auth_failed");

        // We must sign out of the external cookie so it doesn't linger
        await HttpContext.SignOutAsync("ExternalCookie");

        var provider = result.Properties?.Items.ContainsKey("provider") == true ? result.Properties.Items["provider"] : (result.Principal.Identity?.AuthenticationType ?? "Unknown");
        var accessToken = result.Properties?.GetTokenValue("access_token");

        if (string.IsNullOrEmpty(accessToken))
            return Redirect("https://localhost:7180/login?error=token_missing");

        var socialLoginRequest = new
        {
            Provider = provider,
            AccessToken = accessToken
        };

        var client = _httpClientFactory.CreateClient("Host");
        var response = await client.PostAsJsonAsync("/api/identity/auth/social-login", socialLoginRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Social login API call failed. Status: {StatusCode}, Body: {Body}", response.StatusCode, errorBody);
            return Redirect("https://localhost:7180/login?error=server_error");
        }

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
        
        if (loginResponse == null)
        {
            _logger.LogError("Social login API call succeeded but response body was null or invalid.");
            return Redirect("https://localhost:7180/login?error=invalid_response");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, loginResponse.User.Id.ToString()),
            new Claim(ClaimTypes.Email, loginResponse.User.Email),
            new Claim(ClaimTypes.Role, loginResponse.User.Role)
        };

        if (!string.IsNullOrEmpty(loginResponse.User.FirstName))
            claims.Add(new Claim(ClaimTypes.GivenName, loginResponse.User.FirstName));
            
        if (!string.IsNullOrEmpty(loginResponse.User.LastName))
            claims.Add(new Claim(ClaimTypes.Surname, loginResponse.User.LastName));
            
        if (!string.IsNullOrEmpty(loginResponse.User.ProfileImageUrl))
            claims.Add(new Claim("ProfileImageUrl", loginResponse.User.ProfileImageUrl));

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };
        
        authProperties.StoreTokens([
            new AuthenticationToken { Name = "access-token", Value = loginResponse.AccessToken },
            new AuthenticationToken { Name = "refresh-token", Value = loginResponse.RefreshToken },
            new AuthenticationToken { Name = "expires_at", Value = loginResponse.ExpiresAt.ToString("o") }
        ]);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return Redirect("https://localhost:7180/");
    }

    [HttpGet("user")]
    public IActionResult GetCurrentUser()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Unauthorized();

        var returnedUser = new
        {
            Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            Role = User.FindFirst(ClaimTypes.Role)?.Value,
            FirstName = User.FindFirst(ClaimTypes.GivenName)?.Value,
            LastName = User.FindFirst(ClaimTypes.Surname)?.Value,
            ProfileImageUrl = User.FindFirst("ProfileImageUrl")?.Value
        };
        return Ok(returnedUser);
    }
}
