using FluentAssertions;
using RentIt.Modules.Analytics.Domain.Entities;
using Xunit;

namespace RentIt.Modules.Analytics.Tests.Domain;

public class HostMetricsTests
{
    [Fact]
    public void Create_ShouldInitializeMetricsWithZeros()
    {
        // Arrange
        var hostId = Guid.NewGuid();

        // Act
        var metrics = HostMetrics.Create(hostId);

        // Assert
        metrics.Should().NotBeNull();
        metrics.HostId.Should().Be(hostId);
        metrics.TotalProperties.Should().Be(0);
        metrics.TotalBookings.Should().Be(0);
        metrics.TotalRevenue.Should().Be(0);
        metrics.TotalReviews.Should().Be(0);
        metrics.AverageRating.Should().Be(0);
    }

    [Fact]
    public void IncrementProperties_ShouldIncreaseTotalPropertiesByOne()
    {
        // Arrange
        var metrics = HostMetrics.Create(Guid.NewGuid());

        // Act
        metrics.IncrementProperties();

        // Assert
        metrics.TotalProperties.Should().Be(1);
    }

    [Fact]
    public void IncrementBookings_ShouldIncreaseTotalBookingsByOne()
    {
        // Arrange
        var metrics = HostMetrics.Create(Guid.NewGuid());

        // Act
        metrics.IncrementBookings();

        // Assert
        metrics.TotalBookings.Should().Be(1);
    }

    [Fact]
    public void AddRevenue_ShouldIncreaseTotalRevenue()
    {
        // Arrange
        var metrics = HostMetrics.Create(Guid.NewGuid());
        var initialAmount = 100.50m;
        var additionalAmount = 50.25m;
        
        metrics.AddRevenue(initialAmount);

        // Act
        metrics.AddRevenue(additionalAmount);

        // Assert
        metrics.TotalRevenue.Should().Be(initialAmount + additionalAmount);
    }

    [Fact]
    public void AddReview_ShouldCalculateAverageRatingCorrectly()
    {
        // Arrange
        var metrics = HostMetrics.Create(Guid.NewGuid());

        // Act & Assert
        metrics.AddReview(5);
        metrics.TotalReviews.Should().Be(1);
        metrics.AverageRating.Should().Be(5);

        metrics.AddReview(3);
        metrics.TotalReviews.Should().Be(2);
        metrics.AverageRating.Should().Be(4); // (5 + 3) / 2 = 4

        metrics.AddReview(4);
        metrics.TotalReviews.Should().Be(3);
        metrics.AverageRating.Should().BeApproximately(4, 0.001);
        
        metrics.AddReview(1);
        metrics.TotalReviews.Should().Be(4);
        metrics.AverageRating.Should().Be(3.25); // (5 + 3 + 4 + 1) / 4 = 3.25
    }

    [Fact]
    public void AddRevenue_ShouldThrowArgumentOutOfRangeException_WhenAmountIsNegative()
    {
        // Arrange
        var metrics = HostMetrics.Create(Guid.NewGuid());

        // Act
        Action act = () => metrics.AddRevenue(-100m);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Amount cannot be negative.*");
    }

    [Fact]
    public void DeductRevenue_ShouldThrowArgumentOutOfRangeException_WhenAmountIsNegative()
    {
        // Arrange
        var metrics = HostMetrics.Create(Guid.NewGuid());

        // Act
        Action act = () => metrics.DeductRevenue(-100m);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Amount cannot be negative.*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-2)]
    public void AddReview_ShouldThrowArgumentOutOfRangeException_WhenRatingIsInvalid(int invalidRating)
    {
        // Arrange
        var metrics = HostMetrics.Create(Guid.NewGuid());

        // Act
        Action act = () => metrics.AddReview(invalidRating);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Rating must be between 1 and 5.*");
    }
}
