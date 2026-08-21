using FluentAssertions;
using Moq;
using RentIt.Modules.Analytics.Application.EventHandlers;
using RentIt.Modules.Analytics.Domain.Entities;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Contracts.Properties.IntegrationEvents;

namespace RentIt.Modules.Analytics.Tests.Application.EventHandlers;

public class PropertyCreatedIntegrationEventHandlerTests
{
    private readonly Mock<IPropertyMetricsRepository> _propertyMetricsRepositoryMock;
    private readonly Mock<IHostMetricsRepository> _hostMetricsRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PropertyCreatedIntegrationEventHandler _handler;

    public PropertyCreatedIntegrationEventHandlerTests()
    {
        _propertyMetricsRepositoryMock = new Mock<IPropertyMetricsRepository>();
        _hostMetricsRepositoryMock = new Mock<IHostMetricsRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new PropertyCreatedIntegrationEventHandler(
            _propertyMetricsRepositoryMock.Object,
            _hostMetricsRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenMetricsDoNotExist_ShouldCreateAndAddMetrics()
    {
        // Arrange
        var notification = new PropertyCreatedIntegrationEvent(
            Guid.NewGuid(), // PropertyId
            Guid.NewGuid(), // HostId
            "Test Property",
            "City",
            "Region");

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
            m.HostId == notification.HostId), It.IsAny<CancellationToken>()), Times.Once);

        _hostMetricsRepositoryMock.Verify(r => r.AddAsync(It.Is<HostMetrics>(m => 
            m.HostId == notification.HostId && 
            m.TotalProperties == 1), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenMetricsExist_ShouldUpdateMetrics()
    {
        // Arrange
        var notification = new PropertyCreatedIntegrationEvent(
            Guid.NewGuid(), // PropertyId
            Guid.NewGuid(), // HostId
            "Test Property",
            "City",
            "Region");

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
        // Property is already tracked, so it won't add again, and there's no update for properties being created if it exists.
        _propertyMetricsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<PropertyMetrics>(), It.IsAny<CancellationToken>()), Times.Never);
        _propertyMetricsRepositoryMock.Verify(r => r.Update(It.IsAny<PropertyMetrics>()), Times.Never);

        _hostMetricsRepositoryMock.Verify(r => r.Update(existingHostMetrics), Times.Once);
        existingHostMetrics.TotalProperties.Should().Be(1);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
