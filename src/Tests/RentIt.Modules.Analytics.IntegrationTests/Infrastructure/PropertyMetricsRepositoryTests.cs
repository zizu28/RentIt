using FluentAssertions;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Infrastructure.Repositories;
using Xunit;

namespace RentIt.Modules.Analytics.IntegrationTests.Infrastructure;

public class PropertyMetricsRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldAddPropertyMetricsToDatabase()
    {
        // Arrange
        var repository = new PropertyMetricsRepository(DbContext);
        var metrics = PropertyMetrics.Create(Guid.NewGuid(), Guid.NewGuid());
        metrics.AddRevenue(500m);
        metrics.IncrementBookings();

        // Act
        await repository.AddAsync(metrics, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Assert
        var dbMetrics = await repository.GetByPropertyIdAsync(metrics.PropertyId, CancellationToken.None);
        dbMetrics.Should().NotBeNull();
        dbMetrics!.TotalBookings.Should().Be(1);
        dbMetrics.TotalRevenue.Should().Be(500m);
    }
}
