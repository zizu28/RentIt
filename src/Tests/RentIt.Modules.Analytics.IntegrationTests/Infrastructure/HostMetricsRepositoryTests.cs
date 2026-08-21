using FluentAssertions;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Infrastructure.Repositories;
using Xunit;

namespace RentIt.Modules.Analytics.IntegrationTests.Infrastructure;

public class HostMetricsRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldAddHostMetricsToDatabase()
    {
        // Arrange
        var repository = new HostMetricsRepository(DbContext);
        var metrics = HostMetrics.Create(Guid.NewGuid());
        metrics.AddRevenue(1200m);
        metrics.IncrementProperties();

        // Act
        await repository.AddAsync(metrics, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Assert
        var dbMetrics = await repository.GetByHostIdAsync(metrics.HostId, CancellationToken.None);
        dbMetrics.Should().NotBeNull();
        dbMetrics!.TotalProperties.Should().Be(1);
        dbMetrics.TotalRevenue.Should().Be(1200m);
    }
}
