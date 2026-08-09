using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Modules.Bookings.Infrastructure.Database;

namespace RentIt.Modules.Bookings.Infrastructure.Repositories;

internal sealed class BookablePropertyRepository(BookingsDbContext context) : IBookablePropertyRepository
{
    private readonly BookingsDbContext _context = context;

    public async Task<BookableProperty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BookableProperties.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<BookableProperty>> GetByHostIdAsync(Guid hostId, CancellationToken cancellationToken = default)
    {
        return await _context.BookableProperties
            .Where(p => p.HostId == hostId)
            .ToListAsync(cancellationToken);
    }

    public void Add(BookableProperty property)
    {
        _context.BookableProperties.Add(property);
    }

    public void Update(BookableProperty property)
    {
        _context.BookableProperties.Update(property);
    }

    public void Remove(BookableProperty property)
    {
        _context.BookableProperties.Remove(property);
    }
}
