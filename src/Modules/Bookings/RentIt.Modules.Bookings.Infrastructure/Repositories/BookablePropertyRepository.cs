using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Modules.Bookings.Infrastructure.Database;

namespace RentIt.Modules.Bookings.Infrastructure.Repositories;

internal sealed class BookablePropertyRepository : IBookablePropertyRepository
{
    private readonly BookingsDbContext _context;

    public BookablePropertyRepository(BookingsDbContext context)
    {
        _context = context;
    }

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
}
