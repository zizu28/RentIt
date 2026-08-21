using FluentAssertions;
using Moq;
using RentIt.Modules.Analytics.Application.Queries;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using Xunit;

namespace RentIt.Modules.Analytics.Tests.Application.Queries;

public class GetPropertyStatsQueryHandlerTests
{
    private readonly Mock<IPropertyMetricsRepository> _repositoryMock;
    private readonly GetPropertyStatsQueryHandler _handler;

    public GetPropertyStatsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IPropertyMetricsRepository>();
        _handler = new GetPropertyStatsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyStats_WhenMetricsNotFound()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var query = new GetPropertyStatsQuery(propertyId);
        
        _repositoryMock.Setup(repo => repo.GetByPropertyIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyMetrics?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PropertyId.Should().Be(propertyId);
        result.Value.TotalBookings.Should().Be(0);
        result.Value.TotalReviews.Should().Be(0);
        result.Value.AverageRating.Should().Be(0.0);
    }

    [Fact]
    public async Task Handle_ShouldReturnStats_WhenMetricsExist()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var hostId = Guid.NewGuid();
        var metrics = PropertyMetrics.Create(propertyId, hostId);
        
        metrics.IncrementBookings(); // 1
        metrics.IncrementBookings(); // 2
        metrics.AddReview(4);
        metrics.AddReview(5);

        var query = new GetPropertyStatsQuery(propertyId);
        
        _repositoryMock.Setup(repo => repo.GetByPropertyIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(metrics);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PropertyId.Should().Be(propertyId);
        result.Value.TotalBookings.Should().Be(2);
        result.Value.TotalReviews.Should().Be(2);
        result.Value.AverageRating.Should().Be(4.5);
    }
}
