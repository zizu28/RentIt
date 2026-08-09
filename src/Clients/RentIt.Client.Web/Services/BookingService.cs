using System.Net.Http.Json;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Client.Web.Services;

public class BookingService(HttpClient httpClient) : IBookingService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IEnumerable<BookingDto>> GetMyBookingsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingDto>>("api/bookings/my-bookings") 
               ?? [];
    }

    public async Task<BookingDto> CreateBookingAsync(Guid propertyId, DateOnly startDate, DateOnly endDate)
    {
        var request = new CreateBookingRequest(propertyId, startDate, endDate);
        var response = await _httpClient.PostAsJsonAsync("api/bookings", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BookingDto>();
        return result ?? throw new Exception("Failed to deserialize the created booking.");
    }

    public async Task<IEnumerable<BookedPeriodDto>> GetPropertyBookedPeriodsAsync(Guid propertyId)
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookedPeriodDto>>($"api/bookings/properties/{propertyId}/booked-periods") 
               ?? [];
    }

    public async Task CreateBookablePropertyAsync(Guid propertyId, string title, string imageUrl, decimal pricePerNight, string currency = "GHS")
    {
        var request = new { PropertyId = propertyId, Title = title, ImageUrl = imageUrl, PricePerNight = pricePerNight, Currency = currency };
        // We catch errors because if it already exists, the API will return BadRequest which throws on EnsureSuccessStatusCode.
        // It's acceptable if the BookableProperty is already created.
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/bookings/properties", request);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            // Ignore HTTP errors for now (e.g. 400 Bad Request if already exists)
        }
    }

    public async Task<IEnumerable<BookingDto>> GetHostPendingBookingsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingDto>>("api/bookings/host/pending-payments") 
               ?? [];
    }

    public async Task RescindBookingAsync(Guid bookingId)
    {
        var response = await _httpClient.PostAsync($"api/bookings/{bookingId}/rescind", null);
        response.EnsureSuccessStatusCode();
    }
}

public record CreateBookingRequest(Guid PropertyId, DateOnly StartDate, DateOnly EndDate);
