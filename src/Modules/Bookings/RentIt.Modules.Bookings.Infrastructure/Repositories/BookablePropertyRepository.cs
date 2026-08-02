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
        return await _context.BookableProperties.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
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
