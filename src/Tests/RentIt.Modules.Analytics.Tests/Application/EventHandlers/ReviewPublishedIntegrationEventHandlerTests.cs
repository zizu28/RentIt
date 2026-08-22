using FluentAssertions;
using Moq;
using RentIt.Modules.Analytics.Application.EventHandlers;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Reviews.IntegrationEvents;

namespace RentIt.Modules.Analytics.Tests.Application.EventHandlers;

public class ReviewPublishedIntegrationEventHandlerTests
{
    private readonly Mock<IPropertyMetricsRepository> _propertyMetricsRepositoryMock;
    private readonly Mock<IHostMetricsRepository> _hostMetricsRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ReviewPublishedIntegrationEventHandler _handler;

    public ReviewPublishedIntegrationEventHandlerTests()
    {
        _propertyMetricsRepositoryMock = new Mock<IPropertyMetricsRepository>();
        _hostMetricsRepositoryMock = new Mock<IHostMetricsRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new ReviewPublishedIntegrationEventHandler(
            _propertyMetricsRepositoryMock.Object,
            _hostMetricsRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenPropertyMetricsExist_ShouldUpdateMetricsAndHostMetrics()
    {
        // Arrange
        var hostId = Guid.NewGuid();
        var notification = new ReviewPublishedIntegrationEvent(
            Guid.NewGuid(), // ReviewId
            Guid.NewGuid(), // BookingId
            Guid.NewGuid(), // PropertyId
            hostId,         // HostId
            Guid.NewGuid(), // ReviewerId
            5,
            "Great stay!");


        var existingPropertyMetrics = PropertyMetrics.Create(notification.PropertyId, hostId);
        
        var existingHostMetrics = HostMetrics.Create(hostId);

        _propertyMetricsRepositoryMock
            .Setup(r => r.GetByPropertyIdAsync(notification.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPropertyMetrics);

        _hostMetricsRepositoryMock
            .Setup(r => r.GetByHostIdAsync(hostId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingHostMetrics);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _propertyMetricsRepositoryMock.Verify(r => r.Update(existingPropertyMetrics), Times.Once);
        existingPropertyMetrics.TotalReviews.Should().Be(1);
        existingPropertyMetrics.AverageRating.Should().Be(5);

        _hostMetricsRepositoryMock.Verify(r => r.Update(existingHostMetrics), Times.Once);
        existingHostMetrics.TotalReviews.Should().Be(1);
        existingHostMetrics.AverageRating.Should().Be(5);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPropertyMetricsExistButHostMetricsDoNotExist_ShouldCreateHostMetrics()
    {
        // Arrange
        var hostId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var notification = new ReviewPublishedIntegrationEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            propertyId,
            hostId,
            Guid.NewGuid(),
            4,
            "Good stay!");

        var existingPropertyMetrics = PropertyMetrics.Create(notification.PropertyId, hostId);

        _propertyMetricsRepositoryMock
            .Setup(r => r.GetByPropertyIdAsync(notification.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPropertyMetrics);

        _hostMetricsRepositoryMock
            .Setup(r => r.GetByHostIdAsync(hostId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HostMetrics?)null);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _propertyMetricsRepositoryMock.Verify(r => r.Update(existingPropertyMetrics), Times.Once);
        
        _hostMetricsRepositoryMock.Verify(r => r.AddAsync(It.Is<HostMetrics>(m => 
            m.HostId == hostId && 
            m.TotalReviews == 1 &&
            m.AverageRating == 4), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPropertyMetricsDoNotExist_ShouldNotDoAnything()
    {
        // Arrange
        var notification = new ReviewPublishedIntegrationEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            null);

        _propertyMetricsRepositoryMock
            .Setup(r => r.GetByPropertyIdAsync(notification.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyMetrics?)null);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _propertyMetricsRepositoryMock.Verify(r => r.Update(It.IsAny<PropertyMetrics>()), Times.Never);
        _hostMetricsRepositoryMock.Verify(r => r.Update(It.IsAny<HostMetrics>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
