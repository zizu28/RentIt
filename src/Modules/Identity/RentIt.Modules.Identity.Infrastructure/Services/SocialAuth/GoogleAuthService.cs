using System.Net.Http.Headers;
using System.Net.Http.Json;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Modules.Identity.Application.Models;
using RentIt.Shared.Abstractions.Results;
using System.Text.Json.Serialization;

namespace RentIt.Modules.Identity.Infrastructure.Services.SocialAuth;

public sealed class GoogleAuthService(HttpClient httpClient) : ISocialAuthService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Result<SocialUserProfile>> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            // Call Google UserInfo endpoint
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Google", "Failed to retrieve user info from Google."));
            }

            var userInfo = await response.Content.ReadFromJsonAsync<GoogleUserInfoResponse>(cancellationToken: cancellationToken);

            if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.Email))
            {
                return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Google", "Google account does not have an email address associated."));
            }

            var profile = new SocialUserProfile(
                Provider: "Google",
                ProviderId: userInfo.Sub,
                Email: userInfo.Email,
                FirstName: userInfo.GivenName ?? string.Empty,
                LastName: userInfo.FamilyName ?? string.Empty,
                ProfileImageUrl: userInfo.Picture,
                Address: userInfo.Locale
            );

            return Result.Success(profile);
        }
        catch (Exception ex)
        {
            return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Google", $"Google authentication failed: {ex.Message}"));
        }
    }

    private class GoogleUserInfoResponse
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; }

        [JsonPropertyName("family_name")]
        public string? FamilyName { get; set; }

        [JsonPropertyName("picture")]
        public string? Picture { get; set; }

        [JsonPropertyName("locale")]
        public string? Locale { get; set; }
    }
}
