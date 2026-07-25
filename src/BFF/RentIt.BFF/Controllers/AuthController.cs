using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RentIt.Shared.DTOs.Identity;

namespace RentIt.BFF.Controllers;

[ApiController]
[Route("bff/auth")]
public sealed class AuthController(IHttpClientFactory httpClientFactory) : ControllerBase
{
#pragma warning disable
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("Gateway");

        var response = await client.PostAsJsonAsync("/identity/auth/login", request, cancellationToken);

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

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Store the JWT in the authentication properties to be extracted by YARP
        var tokenAuthProperties = new AuthenticationProperties();
        tokenAuthProperties.StoreTokens([
            new AuthenticationToken { Name = "access-token", Value = loginResponse.AccessToken }
        ]);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            tokenAuthProperties);

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
        return Challenge(properties, provider);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(CancellationToken cancellationToken)
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded || result.Principal == null)
            return Redirect("https://localhost:7180/login?error=auth_failed");


        var provider = result.Principal.Identity?.AuthenticationType ?? "Unknown";
        var accessToken = result.Properties?.GetTokenValue("access_token");

        if (string.IsNullOrEmpty(accessToken))
            return Redirect("https://localhost:7180/login?error=token_missing");

        var socialLoginRequest = new
        {
            Provider = provider,
            AccessToken = accessToken
        };

        var client = _httpClientFactory.CreateClient("Gateway");
        var response = await client.PostAsJsonAsync("/identity/auth/social-login", socialLoginRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return Redirect("https://localhost:7180/login?error=server_error");

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
        
        if (loginResponse == null)
            return Redirect("https://localhost:7180/login?error=invalid_response");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, loginResponse.User.Id.ToString()),
            new Claim(ClaimTypes.Email, loginResponse.User.Email),
            new Claim(ClaimTypes.Role, loginResponse.User.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };
        
        authProperties.StoreTokens([
            new AuthenticationToken { Name = "access-token", Value = loginResponse.AccessToken }
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
            Role = User.FindFirst(ClaimTypes.Role)?.Value
        };
        return Ok(new { user = returnedUser });
    }
}
