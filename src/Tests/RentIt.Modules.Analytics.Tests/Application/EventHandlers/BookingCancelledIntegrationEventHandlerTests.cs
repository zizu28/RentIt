using FluentAssertions;
using Moq;
using RentIt.Modules.Analytics.Application.EventHandlers;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Bookings.IntegrationEvents;

namespace RentIt.Modules.Analytics.Tests.Application.EventHandlers;

public class BookingCancelledIntegrationEventHandlerTests
{
    private readonly Mock<IPropertyMetricsRepository> _propertyMetricsRepositoryMock;
    private readonly Mock<IHostMetricsRepository> _hostMetricsRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly BookingCancelledIntegrationEventHandler _handler;

    public BookingCancelledIntegrationEventHandlerTests()
    {
        _propertyMetricsRepositoryMock = new Mock<IPropertyMetricsRepository>();
        _hostMetricsRepositoryMock = new Mock<IHostMetricsRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new BookingCancelledIntegrationEventHandler(
            _propertyMetricsRepositoryMock.Object,
            _hostMetricsRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenMetricsDoNotExist_ShouldCreateAndAddMetrics()
    {
        // Arrange
        var notification = new BookingCancelledIntegrationEvent(
            Guid.NewGuid(),
            Guid.NewGuid(), // PropertyId
            Guid.NewGuid(),
            Guid.NewGuid(), // HostId
            "Renter",
            "Change of plans",
            50m,
            "USD");

        _propertyMetricsRepositoryMock
            .Setup(r => r.GetByPropertyIdAsync(notification.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyMetrics?)null);

        _hostMetricsRepositoryMock
            .Setup(r => r.GetByHostIdAsync(notification.HostId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HostMetrics?)null);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _propertyMetricsRepositoryMock.Verify(r => r.AddAsync(It.Is<PropertyMetrics>(m => 
            m.PropertyId == notification.PropertyId && 
            m.TotalCancellations == 1 &&
            m.TotalRevenue == -50m), It.IsAny<CancellationToken>()), Times.Once);

        _hostMetricsRepositoryMock.Verify(r => r.AddAsync(It.Is<HostMetrics>(m => 
            m.HostId == notification.HostId && 
            m.TotalRevenue == -50m), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenMetricsExist_ShouldUpdateMetrics()
    {
        // Arrange
        var notification = new BookingCancelledIntegrationEvent(
            Guid.NewGuid(),
            Guid.NewGuid(), // PropertyId
            Guid.NewGuid(),
            Guid.NewGuid(), // HostId
            "Renter",
            "Change of plans",
            50m,
            "USD");

        var existingPropertyMetrics = PropertyMetrics.Create(notification.PropertyId, notification.HostId);
        existingPropertyMetrics.AddRevenue(100m);
        var existingHostMetrics = HostMetrics.Create(notification.HostId);
        existingHostMetrics.AddRevenue(100m);

        _propertyMetricsRepositoryMock
            .Setup(r => r.GetByPropertyIdAsync(notification.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPropertyMetrics);

        _hostMetricsRepositoryMock
            .Setup(r => r.GetByHostIdAsync(notification.HostId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingHostMetrics);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _propertyMetricsRepositoryMock.Verify(r => r.Update(existingPropertyMetrics), Times.Once);
        existingPropertyMetrics.TotalCancellations.Should().Be(1);
        existingPropertyMetrics.TotalRevenue.Should().Be(50m); // 100 - 50

        _hostMetricsRepositoryMock.Verify(r => r.Update(existingHostMetrics), Times.Once);
        existingHostMetrics.TotalRevenue.Should().Be(50m); // 100 - 50

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
