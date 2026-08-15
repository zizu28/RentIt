using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using RentIt.Modules.Payments.Application.Services;

namespace RentIt.Modules.Payments.Infrastructure.Services;

internal sealed class PaystackService : IPaystackService
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;

    public PaystackService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _secretKey = configuration["Paystack:SecretKey"] ?? string.Empty;
        
        _httpClient.BaseAddress = new Uri("https://api.paystack.co");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
    }

    public async Task<InitializeTransactionResponse> InitializeTransactionAsync(InitializeTransactionRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            email = request.Email,
            amount = (long)(request.Amount * 100), // Paystack requires amount in smallest currency unit
            reference = request.Reference,
            callback_url = request.CallbackUrl
        };

        var response = await _httpClient.PostAsJsonAsync("/transaction/initialize", payload, cancellationToken);
        
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<InitializeTransactionResponse>(cancellationToken: cancellationToken);
        return result ?? throw new Exception("Failed to deserialize Paystack response");
    }

    public async Task<VerifyTransactionResponse> VerifyTransactionAsync(string reference, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/transaction/verify/{Uri.EscapeDataString(reference)}", cancellationToken);
        
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VerifyTransactionResponse>(cancellationToken: cancellationToken);
        return result ?? throw new Exception("Failed to deserialize Paystack verification response");
    }
}
