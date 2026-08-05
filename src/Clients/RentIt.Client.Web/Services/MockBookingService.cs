using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Client.Web.Services;

public class MockBookingService : IBookingService
{
    private readonly IPropertyService _propertyService;
    private readonly List<BookingDto> _bookings = new();

    public MockBookingService(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    public async Task<IEnumerable<BookingDto>> GetMyBookingsAsync()
    {
        await Task.Delay(400); // Simulate network
        return _bookings;
    }

    public async Task<BookingDto> CreateBookingAsync(Guid propertyId, DateOnly startDate, DateOnly endDate)
    {
        await Task.Delay(600); // Simulate processing

        var property = await _propertyService.GetPropertyByIdAsync(propertyId);
        if (property == null) throw new Exception("Property not found");

        int nights = endDate.DayNumber - startDate.DayNumber;
        decimal total = property.PricePerNight * nights;

        var booking = new BookingDto
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            PropertyTitle = property.Title,
            PropertyImageUrl = property.ImageUrls.FirstOrDefault() ?? "",
            StartDate = startDate,
            EndDate = endDate,
            TotalPrice = total,
            Status = "Confirmed"
        };

        _bookings.Add(booking);
        return booking;
    }

    public async Task<IEnumerable<BookedPeriodDto>> GetPropertyBookedPeriodsAsync(Guid propertyId)
    {
        await Task.Delay(200); // Simulate network
        return _bookings
            .Where(b => b.PropertyId == propertyId && b.Status != "Cancelled")
            .Select(b => new BookedPeriodDto(b.StartDate, b.EndDate))
            .ToList();
    }

    public Task CreateBookablePropertyAsync(Guid propertyId, string title, string imageUrl, decimal pricePerNight, string currency = "GHS")
    {
        // Mock implementation - do nothing
        return Task.CompletedTask;
    }
}
