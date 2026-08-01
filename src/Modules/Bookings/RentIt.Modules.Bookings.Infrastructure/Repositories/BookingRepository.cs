using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Modules.Bookings.Infrastructure.Database;

namespace RentIt.Modules.Bookings.Infrastructure.Repositories;

internal sealed class BookingRepository : IBookingRepository
{
    private readonly BookingsDbContext _context;

    public BookingRepository(BookingsDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByGuestIdAsync(Guid guestId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.GuestId == guestId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.PropertyId == propertyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingBookingsAsync(Guid propertyId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.AnyAsync(
            b => b.PropertyId == propertyId &&
                 b.Status != BookingStatus.Cancelled &&
                 b.StartDate < endDate &&
                 b.EndDate > startDate,
            cancellationToken);
    }

    public void Add(Booking booking)
    {
        _context.Bookings.Add(booking);
    }

    public void Update(Booking booking)
    {
        _context.Bookings.Update(booking);
    }
}
