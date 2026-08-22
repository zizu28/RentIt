using Moq;
using MediatR;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Contracts.Identity.Queries;
using RentIt.Shared.Abstractions.Results;
using RentIt.Shared.Abstractions.Email;
using System.Linq.Expressions;
using Serilog;
using Xunit;

namespace RentIt.Modules.Bookings.Tests.Handlers;

public class RemindGuestPaymentCommandTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IBookablePropertyRepository> _propertyRepositoryMock;
    private readonly Mock<IBackgroundJob> _backgroundJobMock;
    private readonly Mock<ISender> _senderMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly RemindGuestPaymentCommandHandler _handler;

    public RemindGuestPaymentCommandTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _propertyRepositoryMock = new Mock<IBookablePropertyRepository>();
        _backgroundJobMock = new Mock<IBackgroundJob>();
        _senderMock = new Mock<ISender>();
        _loggerMock = new Mock<ILogger>();

        _handler = new RemindGuestPaymentCommandHandler(
            _bookingRepositoryMock.Object,
            _propertyRepositoryMock.Object,
            _loggerMock.Object,
            _backgroundJobMock.Object,
            _senderMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_EnqueuesEmail()
    {
        // Arrange
        var command = new RemindGuestPaymentCommand(Guid.NewGuid(), Guid.NewGuid());
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

        _senderMock.Setup(s => s.Send(It.IsAny<GetUserEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success("test@example.com"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _backgroundJobMock.Verify(j => j.Enqueue<IEmailService>(
            It.IsAny<string>(), 
            It.IsAny<Expression<Func<IEmailService, Task>>>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookingNotFound_DoesNothing()
    {
        // Arrange
        var command = new RemindGuestPaymentCommand(Guid.NewGuid(), Guid.NewGuid());

        _bookingRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking)null!);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _backgroundJobMock.Verify(j => j.Enqueue<IEmailService>(
            It.IsAny<string>(), 
            It.IsAny<Expression<Func<IEmailService, Task>>>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenHostIsNotOwner_DoesNothing()
    {
        // Arrange
        var command = new RemindGuestPaymentCommand(Guid.NewGuid(), Guid.NewGuid());
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
            Guid.NewGuid()); // Owner is different

        _bookingRepositoryMock.Setup(repo => repo.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _backgroundJobMock.Verify(j => j.Enqueue<IEmailService>(
            It.IsAny<string>(), 
            It.IsAny<Expression<Func<IEmailService, Task>>>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBookingIsNotPending_DoesNothing()
    {
        // Arrange
        var command = new RemindGuestPaymentCommand(Guid.NewGuid(), Guid.NewGuid());
        var propertyId = Guid.NewGuid();

        var booking = Booking.Create(
            propertyId,
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
            100,
            "USD",
            1);

        booking.Confirm(); // Status is now Confirmed

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
        _backgroundJobMock.Verify(j => j.Enqueue<IEmailService>(
            It.IsAny<string>(), 
            It.IsAny<Expression<Func<IEmailService, Task>>>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenGuestEmailNotFound_DoesNotEnqueueEmail()
    {
        // Arrange
        var command = new RemindGuestPaymentCommand(Guid.NewGuid(), Guid.NewGuid());
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

        _senderMock.Setup(s => s.Send(It.IsAny<GetUserEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<string>("User not found"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _backgroundJobMock.Verify(j => j.Enqueue<IEmailService>(
            It.IsAny<string>(), 
            It.IsAny<Expression<Func<IEmailService, Task>>>()), 
            Times.Never);
    }
}
