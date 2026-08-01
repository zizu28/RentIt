using RentIt.Modules.Bookings.Domain.Entities;

namespace RentIt.Modules.Bookings.Domain.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByGuestIdAsync(Guid guestId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingBookingsAsync(Guid propertyId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    void Add(Booking booking);
    void Update(Booking booking);
}
