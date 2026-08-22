using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Bookings.Infrastructure.Database;
using Testcontainers.MsSql;
using Xunit;

namespace RentIt.Modules.Bookings.IntegrationTests;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    protected BookingsDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var options = new DbContextOptionsBuilder<BookingsDbContext>()
            .UseSqlServer(_dbContainer.GetConnectionString())
            .Options;

        DbContext = new BookingsDbContext(options);
        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }
}
