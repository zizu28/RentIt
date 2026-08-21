using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Shared.Infrastructure.Messaging;

namespace RentIt.Modules.Analytics.Infrastructure.Database;

public class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : DbContext(options)
{
    public DbSet<PropertyMetrics> PropertyMetrics { get; set; } = null!;
    public DbSet<HostMetrics> HostMetrics { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("analytics");

        modelBuilder.Entity<PropertyMetrics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PropertyId).IsRequired();
            entity.HasIndex(e => e.PropertyId).IsUnique();
            entity.Property(e => e.HostId).IsRequired();
            entity.HasIndex(e => e.HostId);
            entity.Property(e => e.TotalBookings).IsRequired();
            entity.Property(e => e.TotalCancellations).IsRequired();
            entity.Property(e => e.TotalRevenue).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.TotalReviews).IsRequired();
            entity.Property(e => e.AverageRating).IsRequired();
        });

        modelBuilder.Entity<HostMetrics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HostId).IsRequired();
            entity.HasIndex(e => e.HostId).IsUnique();
            entity.Property(e => e.TotalProperties).IsRequired();
            entity.Property(e => e.TotalBookings).IsRequired();
            entity.Property(e => e.TotalRevenue).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.TotalReviews).IsRequired();
            entity.Property(e => e.AverageRating).IsRequired();
        });

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
    }
}
