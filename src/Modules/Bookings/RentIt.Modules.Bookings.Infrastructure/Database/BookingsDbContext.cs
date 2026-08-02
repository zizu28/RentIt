using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Infrastructure.Messaging;

namespace RentIt.Modules.Bookings.Infrastructure.Database;

public class BookingsDbContext(DbContextOptions<BookingsDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookableProperty> BookableProperties { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("bookings");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingsDbContext).Assembly);
        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
