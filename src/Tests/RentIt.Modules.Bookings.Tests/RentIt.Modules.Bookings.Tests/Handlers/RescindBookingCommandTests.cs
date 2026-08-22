using FluentAssertions;
using Moq;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using Serilog;
using Xunit;

namespace RentIt.Modules.Bookings.Tests.Handlers;

public class RescindBookingCommandTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IBookablePropertyRepository> _propertyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RescindBookingCommandHandler _handler;

    public RescindBookingCommandTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _propertyRepositoryMock = new Mock<IBookablePropertyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger>();

        _handler = new RescindBookingCommandHandler(
            _bookingRepositoryMock.Object,
            _propertyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_RescindsBooking()
    {
        // Arrange
        var command = new RescindBookingCommand(Guid.NewGuid(), Guid.NewGuid());
        var propertyId = Guid.NewGuid();

        var booking = Booking.Create(
            propertyId,
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
            100,
            "USD",
            1);

        var property = new BookableProperty(
            propertyId,
            "Awesome Villa",
            "http://image.com/villa.jpg",
            100,
            "USD",
            1,
            command.HostId);

        _bookingRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        booking.Status.Should().Be(BookingStatus.Cancelled);
        _bookingRepositoryMock.Verify(repo => repo.Update(booking), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookingNotFound_ThrowsBookingDomainException()
    {
        // Arrange
        var command = new RescindBookingCommand(Guid.NewGuid(), Guid.NewGuid());

        _bookingRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookingDomainException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain($"Booking with ID {command.BookingId} not found.");

        _bookingRepositoryMock.Verify(repo => repo.Update(It.IsAny<Booking>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenHostIsNotOwner_ThrowsBookingDomainException()
    {
        // Arrange
        var command = new RescindBookingCommand(Guid.NewGuid(), Guid.NewGuid()); // command.HostId here is wrong
        var actualHostId = Guid.NewGuid(); // Different host ID
        var propertyId = Guid.NewGuid();
        
        var booking = Booking.Create(
            propertyId,
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
            100,
            "USD",
            1);

        var property = new BookableProperty(
            propertyId,
            "Awesome Villa",
            "http://image.com/villa.jpg",
            100,
            "USD",
            1,
            actualHostId); // Owner is actualHostId

        _bookingRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookingDomainException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("You are not authorized to cancel this booking.");

        _bookingRepositoryMock.Verify(repo => repo.Update(It.IsAny<Booking>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
