using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Properties.Domain.Entities;
using RentIt.Shared.Infrastructure.Messaging;

namespace RentIt.Modules.Properties.Infrastructure.Database;

public class PropertiesDbContext(DbContextOptions<PropertiesDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("properties");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PropertiesDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
