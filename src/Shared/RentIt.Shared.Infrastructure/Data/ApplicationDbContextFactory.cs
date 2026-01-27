using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RentIt.Shared.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        Console.WriteLine("Creating ApplicationDbContext for design-time operations...");

        var basePath = AppContext.BaseDirectory;

        var hostProjectPath = Path.GetFullPath(Path.Combine(basePath,
            "..", "..", "..", "..", "Host", "RentIt.Host"));

        Console.WriteLine($"Looking for Host project at: {hostProjectPath}");

        if (!Directory.Exists(hostProjectPath))
        {
            throw new InvalidOperationException(
                        $"Could not find Host project. Checked paths:\n" +
                        $"- {Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", "Host", "RentIt.Host"))}\n" +
                        $"- {hostProjectPath}\n" +
                        "Ensure the Host project exists and the relative path is correct.");
        }

        Console.WriteLine($"Using Host project path: {hostProjectPath}");

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        Console.WriteLine($"Environment: {environment}");

        // Change to the host project directory for configuration file resolution
        var originalDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(hostProjectPath);

        try
        {
            var connectionStringName = "DefaultConnection";
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"Connection string '{connectionStringName}' not found in configuration. " +
                    $"Please ensure it's defined in appsettings.json at {hostProjectPath}");
            }

            Console.WriteLine($"Using connection string: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
        finally
        {
            // Restore the original directory
            Directory.SetCurrentDirectory(originalDirectory);
        }
    }
}
