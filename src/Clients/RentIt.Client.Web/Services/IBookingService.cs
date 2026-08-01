using RentIt.Shared.DTOs.Bookings;

namespace RentIt.Client.Web.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetMyBookingsAsync();
    Task<BookingDto> CreateBookingAsync(Guid propertyId, DateOnly startDate, DateOnly endDate);
    Task<IEnumerable<BookedPeriodDto>> GetPropertyBookedPeriodsAsync(Guid propertyId);
}
