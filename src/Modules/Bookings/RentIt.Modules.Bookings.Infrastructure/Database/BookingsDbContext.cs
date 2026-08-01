using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Bookings.Domain.Entities;

namespace RentIt.Modules.Bookings.Infrastructure.Database;

public class BookingsDbContext : DbContext
{
    public BookingsDbContext(DbContextOptions<BookingsDbContext> options) : base(options)
    {
    }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookableProperty> BookableProperties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("bookings");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingsDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
