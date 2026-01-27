using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Identity.Domain.Entities;

namespace RentIt.Shared.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var applicationsToScan = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assembly.FullName != null &&
                assembly.FullName!.StartsWith("RentIt.Modules.Identity.Infrastructure.Configurations"));
        foreach (var assembly in applicationsToScan)
        {
            try
            {
                Console.WriteLine($"Attempting to apply configurations from assembly: {assembly.FullName}");
                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying configurations from assembly {assembly.FullName}: {ex.Message}");
            }
        }
    }
}
