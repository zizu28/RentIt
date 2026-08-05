using MediatR;
using System.Text.Json.Serialization;

namespace RentIt.Modules.Payments.Application.Commands;

public class PaystackWebhookPayload
{
    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public PaystackWebhookData Data { get; set; } = new();
}

public class PaystackWebhookData
{
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }
}

public record ProcessPaymentWebhookCommand(PaystackWebhookPayload Payload) : IRequest;
