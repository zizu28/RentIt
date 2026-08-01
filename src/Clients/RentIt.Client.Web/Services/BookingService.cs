using System.Net.Http.Json;
using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Client.Web.Services;

public class BookingService(HttpClient httpClient) : IBookingService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IEnumerable<BookingDto>> GetMyBookingsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<BookingDto>>("api/bookings/my-bookings") 
               ?? Enumerable.Empty<BookingDto>();
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
               ?? Enumerable.Empty<BookedPeriodDto>();
    }
}

public record CreateBookingRequest(Guid PropertyId, DateOnly StartDate, DateOnly EndDate);
