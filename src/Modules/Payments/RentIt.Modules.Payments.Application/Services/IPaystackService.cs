using System.Text.Json.Serialization;

namespace RentIt.Modules.Payments.Application.Services;

public class InitializeTransactionRequest
{
    public string Email { get; set; } = string.Empty;
    public decimal Amount { get; set; } // Paystack requires amount in kobo/cents. I'll pass decimal and the service will multiply by 100.
    public string Reference { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
}

public class InitializeTransactionResponse
{
    [JsonPropertyName("status")]
    public bool Status { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public InitializeTransactionData Data { get; set; } = new();
}

public class InitializeTransactionData
{
    [JsonPropertyName("authorization_url")]
    public string AuthorizationUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("access_code")]
    public string AccessCode { get; set; } = string.Empty;
    
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;
}

public interface IPaystackService
{
    Task<InitializeTransactionResponse> InitializeTransactionAsync(InitializeTransactionRequest request, CancellationToken cancellationToken = default);
    Task<VerifyTransactionResponse> VerifyTransactionAsync(string reference, CancellationToken cancellationToken = default);
}
