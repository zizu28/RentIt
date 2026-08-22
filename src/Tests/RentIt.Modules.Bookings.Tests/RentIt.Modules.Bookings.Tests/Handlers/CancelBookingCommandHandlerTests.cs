using FluentAssertions;
using Moq;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Application.Handlers;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Exceptions;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using Serilog;
using Xunit;

namespace RentIt.Modules.Bookings.Tests.Handlers;

public class CancelBookingCommandHandlerTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly CancelBookingCommandHandler _handler;

    public CancelBookingCommandHandlerTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger>();

        _handler = new CancelBookingCommandHandler(
            _bookingRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_CancelsBooking()
    {
        // Arrange
        var command = new CancelBookingCommand(Guid.NewGuid(), Guid.NewGuid());
        var propertyId = Guid.NewGuid();

        var booking = Booking.Create(
            propertyId,
            command.GuestId,
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 5),
            100,
            "USD",
            1);

        // Reflection to set the ID (since it's a strongly typed ID or Guid?)
        // Assuming BookingId is a Guid. Wait, let's just let it have a random ID from generation.
        // The command ID might not exactly match the Entity ID if we don't force it, but the mock will return it.
        
        _bookingRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

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
        var command = new CancelBookingCommand(Guid.NewGuid(), Guid.NewGuid());

        _bookingRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookingDomainException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain($"Booking with ID {command.BookingId} not found.");

        _bookingRepositoryMock.Verify(repo => repo.Update(It.IsAny<Booking>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenGuestIsNotOwner_ThrowsBookingDomainException()
    {
        // Arrange
        var command = new CancelBookingCommand(Guid.NewGuid(), Guid.NewGuid()); // GuestId here is wrong
        var actualGuestId = Guid.NewGuid(); // Different guest ID
        
        var booking = Booking.Create(
            Guid.NewGuid(),
            actualGuestId,
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 5),
            100,
            "USD",
            1);

        _bookingRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookingDomainException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("You are not authorized to cancel this booking.");

        _bookingRepositoryMock.Verify(repo => repo.Update(It.IsAny<Booking>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBookingAlreadyCancelled_ThrowsBookingDomainException()
    {
        // Arrange
        var command = new CancelBookingCommand(Guid.NewGuid(), Guid.NewGuid());
        
        var booking = Booking.Create(
            Guid.NewGuid(),
            command.GuestId,
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
            100,
            "USD",
            1);

        booking.Cancel(); // Status becomes Cancelled

        _bookingRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BookingDomainException>(() => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("cannot be cancelled.");

        _bookingRepositoryMock.Verify(repo => repo.Update(It.IsAny<Booking>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
