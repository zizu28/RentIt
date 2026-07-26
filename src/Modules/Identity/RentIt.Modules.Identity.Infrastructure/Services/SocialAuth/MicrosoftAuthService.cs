using System.Net.Http.Headers;
using System.Net.Http.Json;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Modules.Identity.Application.Models;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Identity.Infrastructure.Services.SocialAuth;

public class MicrosoftAuthService : ISocialAuthService
{
    private const string MicrosoftApiUrl = "https://graph.microsoft.com/v1.0/me?$select=id,displayName,mail,userPrincipalName";
    
    private readonly HttpClient _httpClient;

    public MicrosoftAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<SocialUserProfile>> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, MicrosoftApiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Microsoft", "Invalid Microsoft token"));
            }

            var userInfo = await response.Content.ReadFromJsonAsync<MicrosoftUserInfoResponse>(cancellationToken: cancellationToken);

            if (userInfo == null)
            {
                return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Microsoft", "Failed to retrieve user info from Microsoft."));
            }

            var email = !string.IsNullOrWhiteSpace(userInfo.Mail) ? userInfo.Mail : userInfo.UserPrincipalName;

            if (string.IsNullOrWhiteSpace(email))
            {
                return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Microsoft", "Microsoft account does not have an email address associated."));
            }

            // Split displayName into first and last name if possible
            var parts = userInfo.DisplayName?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            var firstName = parts.Length > 0 ? parts[0] : "Unknown";
            var lastName = parts.Length > 1 ? string.Join(' ', parts.Skip(1)) : string.Empty;

            var profile = new SocialUserProfile(
                Provider: "Microsoft",
                ProviderId: userInfo.Id,
                Email: email,
                FirstName: firstName,
                LastName: lastName
            );

            return Result.Success(profile);
        }
        catch (Exception ex)
        {
            return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Microsoft", $"Microsoft authentication failed: {ex.Message}"));
        }
    }

    private class MicrosoftUserInfoResponse
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string UserPrincipalName { get; set; } = string.Empty;
    }
}
