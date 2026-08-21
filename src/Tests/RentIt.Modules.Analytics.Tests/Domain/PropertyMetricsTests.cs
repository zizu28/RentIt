using FluentAssertions;
using RentIt.Modules.Analytics.Domain.Entities;
using Xunit;

namespace RentIt.Modules.Analytics.Tests.Domain;

public class PropertyMetricsTests
{
    [Fact]
    public void Create_ShouldInitializeMetricsWithZeros()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var hostId = Guid.NewGuid();

        // Act
        var metrics = PropertyMetrics.Create(propertyId, hostId);

        // Assert
        metrics.Should().NotBeNull();
        metrics.PropertyId.Should().Be(propertyId);
        metrics.HostId.Should().Be(hostId);
        metrics.TotalBookings.Should().Be(0);
        metrics.TotalCancellations.Should().Be(0);
        metrics.TotalRevenue.Should().Be(0);
        metrics.TotalReviews.Should().Be(0);
        metrics.AverageRating.Should().Be(0);
    }

    [Fact]
    public void IncrementBookings_ShouldIncreaseTotalBookingsByOne()
    {
        // Arrange
        var metrics = PropertyMetrics.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        metrics.IncrementBookings();

        // Assert
        metrics.TotalBookings.Should().Be(1);
    }

    [Fact]
    public void IncrementCancellations_ShouldIncreaseTotalCancellationsByOne()
    {
        // Arrange
        var metrics = PropertyMetrics.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        metrics.IncrementCancellations();

        // Assert
        metrics.TotalCancellations.Should().Be(1);
    }

    [Fact]
    public void AddRevenue_ShouldIncreaseTotalRevenue()
    {
        // Arrange
        var metrics = PropertyMetrics.Create(Guid.NewGuid(), Guid.NewGuid());
        var initialAmount = 200.75m;
        var additionalAmount = 100.50m;
        
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
        var metrics = PropertyMetrics.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        metrics.AddReview(4);
        metrics.TotalReviews.Should().Be(1);
        metrics.AverageRating.Should().Be(4);

        metrics.AddReview(2);
        metrics.TotalReviews.Should().Be(2);
        metrics.AverageRating.Should().Be(3); // (4 + 2) / 2 = 3

        metrics.AddReview(5);
        metrics.TotalReviews.Should().Be(3);
        metrics.AverageRating.Should().BeApproximately(3.666, 0.001); // (4 + 2 + 5) / 3 = 3.666...
    }

    [Fact]
    public void AddRevenue_ShouldThrowArgumentOutOfRangeException_WhenAmountIsNegative()
    {
        // Arrange
        var metrics = PropertyMetrics.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        Action act = () => metrics.AddRevenue(-50m);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Amount cannot be negative.*");
    }

    [Fact]
    public void DeductRevenue_ShouldThrowArgumentOutOfRangeException_WhenAmountIsNegative()
    {
        // Arrange
        var metrics = PropertyMetrics.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        Action act = () => metrics.DeductRevenue(-50m);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Amount cannot be negative.*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void AddReview_ShouldThrowArgumentOutOfRangeException_WhenRatingIsInvalid(int invalidRating)
    {
        // Arrange
        var metrics = PropertyMetrics.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        Action act = () => metrics.AddReview(invalidRating);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Rating must be between 1 and 5.*");
    }
}
