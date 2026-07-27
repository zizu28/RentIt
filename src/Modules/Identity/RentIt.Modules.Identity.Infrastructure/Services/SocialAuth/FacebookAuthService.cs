using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using RentIt.Modules.Identity.Application.Abstractions;
using RentIt.Modules.Identity.Application.Models;
using RentIt.Shared.Abstractions.Results;

namespace RentIt.Modules.Identity.Infrastructure.Services.SocialAuth;

public class FacebookAuthService : ISocialAuthService
{
    private const string FacebookApiUrl = "https://graph.facebook.com/v19.0/me?fields=id,email,first_name,last_name&access_token={0}";
    private const string FacebookTokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
    
    private readonly HttpClient _httpClient;
    private readonly string _appId;
    private readonly string _appSecret;

    public FacebookAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var facebookSettings = configuration.GetSection("Authentication:Facebook");
        _appId = facebookSettings["AppId"] ?? string.Empty;
        _appSecret = facebookSettings["AppSecret"] ?? string.Empty;
    }

    public async Task<Result<SocialUserProfile>> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Validate the token belongs to our App (to prevent confused deputy attack)
            // Skip this if AppId and AppSecret are not configured in this module
            if (!string.IsNullOrEmpty(_appId) && !string.IsNullOrEmpty(_appSecret))
            {
                var tokenValidationUrl = string.Format(FacebookTokenValidationUrl, accessToken, _appId, _appSecret);
                var validationResponse = await _httpClient.GetAsync(tokenValidationUrl, cancellationToken);
                
                if (!validationResponse.IsSuccessStatusCode)
                {
                    return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Facebook", "Invalid Facebook token"));
                }
                
                var validationData = await validationResponse.Content.ReadFromJsonAsync<FacebookTokenValidationResponse>(cancellationToken: cancellationToken);
                
                if (validationData?.Data == null || validationData.Data.AppId != _appId || !validationData.Data.IsValid)
                {
                    return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Facebook", "Facebook token does not belong to this application or is invalid."));
                }
            }

            // 2. Get user info
            var userInfoUrl = string.Format(FacebookApiUrl, accessToken);
            var userInfoResponse = await _httpClient.GetAsync(userInfoUrl, cancellationToken);
            
            if (!userInfoResponse.IsSuccessStatusCode)
            {
                return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Facebook", "Failed to retrieve user info from Facebook."));
            }

            var userInfo = await userInfoResponse.Content.ReadFromJsonAsync<FacebookUserInfoResponse>(cancellationToken: cancellationToken);

            if (userInfo == null || string.IsNullOrWhiteSpace(userInfo.Email))
            {
                return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Facebook", "Facebook account does not have an email address associated."));
            }

            var profile = new SocialUserProfile(
                Provider: "Facebook",
                ProviderId: userInfo.Id,
                Email: userInfo.Email,
                FirstName: userInfo.FirstName,
                LastName: userInfo.LastName
            );

            return Result.Success(profile);
        }
        catch (Exception ex)
        {
            return Result.Failure<SocialUserProfile>(Error.Failure("SocialAuth.Facebook", $"Facebook authentication failed: {ex.Message}"));
        }
    }

    private class FacebookTokenValidationResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("data")]
        public FacebookTokenValidationData Data { get; set; } = null!;
    }

    private class FacebookTokenValidationData
    {
        [System.Text.Json.Serialization.JsonPropertyName("app_id")]
        public string AppId { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }
    }

    private class FacebookUserInfoResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;
    }
}
