using System.Text.Json.Serialization;

namespace RentIt.Modules.Payments.Application.Services;

public class VerifyTransactionResponse
{
    [JsonPropertyName("status")]
    public bool Status { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public VerifyTransactionData Data { get; set; } = new();
}

public class VerifyTransactionData
{
    [JsonPropertyName("amount")]
    public long Amount { get; set; }
    
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("gateway_response")]
    public string GatewayResponse { get; set; } = string.Empty;
    
    [JsonPropertyName("authorization")]
    public VerifyTransactionAuthorization? Authorization { get; set; }
}

public class VerifyTransactionAuthorization
{
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; } = string.Empty;
}
