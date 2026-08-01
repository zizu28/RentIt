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
        return result ?? throw new Exception("Failed to deserialize payment response");
    }
}
