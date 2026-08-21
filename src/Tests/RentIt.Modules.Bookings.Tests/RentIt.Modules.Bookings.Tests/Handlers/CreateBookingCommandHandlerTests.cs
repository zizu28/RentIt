using FluentAssertions;
using Moq;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Application.Handlers;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Kernel.ValueObjects;
using Serilog;
using Xunit;

namespace RentIt.Modules.Bookings.Tests.Handlers;

public class CreateBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IBookablePropertyRepository> _propertyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly CreateBookingCommandHandler _handler;

    public CreateBookingCommandHandlerTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _propertyRepositoryMock = new Mock<IBookablePropertyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger>();

        _handler = new CreateBookingCommandHandler(
            _bookingRepositoryMock.Object,
            _propertyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_CreatesBookingAndReturnsDto()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = new CreateBookingCommand(
            propertyId,
            Guid.NewGuid(),
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 5));

        var property = new BookableProperty(
            propertyId,
            "Awesome Villa",
            "http://image.com/villa.jpg",
            100,
            "USD",
            1,
            Guid.NewGuid());

        _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync(command.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _bookingRepositoryMock.Setup(repo => repo.HasOverlappingBookingsAsync(command.PropertyId, command.StartDate, command.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PropertyId.Should().Be(command.PropertyId);
        result.StartDate.Should().Be(command.StartDate);
        result.EndDate.Should().Be(command.EndDate);

        _bookingRepositoryMock.Verify(repo => repo.Add(It.IsAny<Booking>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPropertyNotFound_ThrowsBookingDomainException()
    {
        // Arrange
        var command = new CreateBookingCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 5));

        _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync(command.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookableProperty)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookingDomainException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain($"Property with ID {command.PropertyId} not found.");

        _bookingRepositoryMock.Verify(repo => repo.Add(It.IsAny<Booking>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOverlappingBookingsExist_ThrowsBookingDomainException()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = new CreateBookingCommand(
            propertyId,
            Guid.NewGuid(),
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 5));

        var property = new BookableProperty(
            propertyId,
            "Awesome Villa",
            "http://image.com/villa.jpg",
            100,
            "USD",
            1,
            Guid.NewGuid());

        _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync(command.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _bookingRepositoryMock.Setup(repo => repo.HasOverlappingBookingsAsync(command.PropertyId, command.StartDate, command.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookingDomainException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("The property is already booked for the selected dates.");

        _bookingRepositoryMock.Verify(repo => repo.Add(It.IsAny<Booking>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
