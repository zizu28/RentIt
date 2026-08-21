using FluentAssertions;
using Moq;
using RentIt.Modules.Analytics.Application.EventHandlers;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Bookings.IntegrationEvents;

namespace RentIt.Modules.Analytics.Tests.Application.EventHandlers;

public class BookingConfirmedIntegrationEventHandlerTests
{
    private readonly Mock<IPropertyMetricsRepository> _propertyMetricsRepositoryMock;
    private readonly Mock<IHostMetricsRepository> _hostMetricsRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly BookingConfirmedIntegrationEventHandler _handler;

    public BookingConfirmedIntegrationEventHandlerTests()
    {
        _propertyMetricsRepositoryMock = new Mock<IPropertyMetricsRepository>();
        _hostMetricsRepositoryMock = new Mock<IHostMetricsRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new BookingConfirmedIntegrationEventHandler(
            _propertyMetricsRepositoryMock.Object,
            _hostMetricsRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenMetricsDoNotExist_ShouldCreateAndAddMetrics()
    {
        // Arrange
        var notification = new BookingConfirmedIntegrationEvent(
            Guid.NewGuid(),
            Guid.NewGuid(), // PropertyId
            Guid.NewGuid(),
            Guid.NewGuid(), // HostId
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 5),
            100m,
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
            m.TotalBookings == 1 &&
            m.TotalRevenue == 100m), It.IsAny<CancellationToken>()), Times.Once);

        _hostMetricsRepositoryMock.Verify(r => r.AddAsync(It.Is<HostMetrics>(m => 
            m.HostId == notification.HostId && 
            m.TotalBookings == 1 &&
            m.TotalRevenue == 100m), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenMetricsExist_ShouldUpdateMetrics()
    {
        // Arrange
        var notification = new BookingConfirmedIntegrationEvent(
            Guid.NewGuid(),
            Guid.NewGuid(), // PropertyId
            Guid.NewGuid(),
            Guid.NewGuid(), // HostId
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 5),
            100m,
            "USD");

        var existingPropertyMetrics = PropertyMetrics.Create(notification.PropertyId, notification.HostId);
        var existingHostMetrics = HostMetrics.Create(notification.HostId);

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
        existingPropertyMetrics.TotalBookings.Should().Be(1);
        existingPropertyMetrics.TotalRevenue.Should().Be(100m);

        _hostMetricsRepositoryMock.Verify(r => r.Update(existingHostMetrics), Times.Once);
        existingHostMetrics.TotalBookings.Should().Be(1);
        existingHostMetrics.TotalRevenue.Should().Be(100m);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
