using System.Net.Http.Json;

namespace RentIt.Client.Web.Services;

public class PaymentService(HttpClient httpClient) : IPaymentService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<InitializePaymentResponseDto> InitializePaymentAsync(InitializePaymentRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/payments/initialize", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<InitializePaymentResponseDto>();
        return result ?? throw new Exception("Failed to deserialize the initialization response.");
    }

    public async Task<PaymentDetailsDto?> GetPaymentByBookingIdAsync(Guid bookingId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PaymentDetailsDto>($"api/payments/booking/{bookingId}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<List<PaymentMethodDto>> GetPaymentMethodsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<PaymentMethodDto>>("api/payments/methods") ?? new();
    }

    public async Task<SetupPaymentMethodResponseDto> SetupPaymentMethodAsync(SetupPaymentMethodRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/payments/methods/setup", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<SetupPaymentMethodResponseDto>();
        return result ?? throw new Exception("Failed to deserialize the setup response.");
    }

    public async Task<bool> VerifyPaymentAsync(string reference)
    {
        var response = await _httpClient.PostAsync($"api/payments/verify/{Uri.EscapeDataString(reference)}", null);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<VerifyPaymentResponseDto>();
        return result?.Success ?? false;
    }
}
