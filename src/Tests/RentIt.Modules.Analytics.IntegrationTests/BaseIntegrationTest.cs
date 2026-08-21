using Microsoft.EntityFrameworkCore;
using RentIt.Modules.Analytics.Infrastructure.Database;
using Testcontainers.MsSql;
using Xunit;

namespace RentIt.Modules.Analytics.IntegrationTests;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    protected AnalyticsDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var options = new DbContextOptionsBuilder<AnalyticsDbContext>()
            .UseSqlServer(_dbContainer.GetConnectionString())
            .Options;

        DbContext = new AnalyticsDbContext(options);
        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }
}
