using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Shared.Infrastructure.Messaging;

namespace RentIt.Modules.Analytics.Infrastructure.Database;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options)
    {
    }

    public DbSet<PropertyMetrics> PropertyMetrics { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("analytics");

        modelBuilder.Entity<PropertyMetrics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PropertyId).IsRequired();
            entity.Property(e => e.TotalBookings).IsRequired();
            entity.Property(e => e.TotalReviews).IsRequired();
            entity.Property(e => e.AverageRating).IsRequired();
        });

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
    }
}
