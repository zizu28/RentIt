using FluentAssertions;
using Moq;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Application.Handlers;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.Persistence;
using RentIt.Shared.Abstractions.Results;
using Serilog;
using Xunit;

namespace RentIt.Modules.Bookings.Tests.Handlers;

public class CreateBookablePropertyCommandHandlerTests
{
    private readonly Mock<IBookablePropertyRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly CreateBookablePropertyCommandHandler _handler;

    public CreateBookablePropertyCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBookablePropertyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger>();

        _handler = new CreateBookablePropertyCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_CreatesPropertyAndReturnsSuccess()
    {
        // Arrange
        var command = new CreateBookablePropertyCommand(
            Guid.NewGuid(),
            "Awesome Villa",
            "http://image.com/villa.jpg",
            100,
            "USD",
            1,
            Guid.NewGuid());

        _repositoryMock.Setup(repo => repo.GetByIdAsync(command.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookableProperty)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(command.PropertyId);
        
        _repositoryMock.Verify(repo => repo.Add(It.IsAny<BookableProperty>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPropertyAlreadyExists_ReturnsFailure()
    {
        // Arrange
        var command = new CreateBookablePropertyCommand(
            Guid.NewGuid(),
            "Awesome Villa",
            "http://image.com/villa.jpg",
            100,
            "USD",
            1,
            Guid.NewGuid());

        var existingProperty = new BookableProperty(
            command.PropertyId,
            command.Title,
            command.ImageUrl,
            command.PricePerNight,
            command.Currency,
            command.RentalPeriod,
            command.HostId);

        _repositoryMock.Setup(repo => repo.GetByIdAsync(command.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Message.Should().Contain("Bookable property already exists.");

        _repositoryMock.Verify(repo => repo.Add(It.IsAny<BookableProperty>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
