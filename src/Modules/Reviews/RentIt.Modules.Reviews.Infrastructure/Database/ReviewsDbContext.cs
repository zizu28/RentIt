using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Reviews.Domain.Entities;

namespace RentIt.Modules.Reviews.Infrastructure.Database;

public sealed class ReviewsDbContext(DbContextOptions<ReviewsDbContext> options) : DbContext(options)
{
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("reviews");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReviewsDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new RentIt.Shared.Infrastructure.Messaging.OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new RentIt.Shared.Infrastructure.Messaging.InboxMessageConfiguration());
    }
}
