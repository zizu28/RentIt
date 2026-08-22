using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RentIt.Modules.Bookings.Application.Commands;
using RentIt.Modules.Bookings.Application.EventHandlers;
using RentIt.Modules.Bookings.Application.Handlers;
using RentIt.Modules.Bookings.Domain.Entities;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Events;
using RentIt.Modules.Bookings.Infrastructure.Database;
using RentIt.Modules.Bookings.Infrastructure.Repositories;
using RentIt.Shared.Abstractions.Messaging;
using RentIt.Shared.Contracts.Bookings.IntegrationEvents;
using Serilog;
using Xunit;

namespace RentIt.Modules.Bookings.IntegrationTests;

public class BookingsIntegrationTests : BaseIntegrationTest
{
    [Fact]
    public async Task CreateBookingCommand_WithValidData_CreatesBookingAndEmitsIntegrationEvent()
    {
        // Arrange
        var hostId = Guid.NewGuid();
        var guestId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();

        var property = new BookableProperty(
            propertyId,
            "Integration Test Villa",
            "http://image.com/villa.jpg",
            150,
            "USD",
            1,
            hostId);

        DbContext.BookableProperties.Add(property);
        await DbContext.SaveChangesAsync();

        var bookingRepository = new BookingRepository(DbContext);
        var propertyRepository = new BookablePropertyRepository(DbContext);
        var loggerMock = new Mock<Serilog.ILogger>();
        var eventBusMock = new Mock<IEventBus>();
        var publisherMock = new Mock<MediatR.IPublisher>();
        var domainEventDispatcher = new DomainEventDispatcher(DbContext, publisherMock.Object);
        var backgroundJobClientMock = new Mock<Hangfire.IBackgroundJobClient>();

        var unitOfWork = new BookingsUnitOfWork(DbContext, domainEventDispatcher, backgroundJobClientMock.Object);

        var createBookingHandler = new CreateBookingCommandHandler(
            bookingRepository,
            propertyRepository,
            unitOfWork,
            loggerMock.Object);

        var command = new CreateBookingCommand(
            propertyId,
            guestId,
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5));

        // Act - Create Booking
        var result = await createBookingHandler.Handle(command, CancellationToken.None);

        // Assert - Creation
        result.Should().NotBeNull();
        
        var savedBooking = await DbContext.Bookings.FirstOrDefaultAsync(b => b.Id == result.Id);
        savedBooking.Should().NotBeNull();
        savedBooking!.Status.Should().Be(BookingStatus.Pending);
        savedBooking.PropertyId.Should().Be(propertyId);
        savedBooking.GuestId.Should().Be(guestId);

        // Act - Domain Event Handling (Simulating EF Core Dispatching / MediatR)
        var domainEvent = new BookingCreatedDomainEvent(
            savedBooking.Id,
            savedBooking.PropertyId,
            savedBooking.GuestId,
            savedBooking.StartDate,
            savedBooking.EndDate,
            savedBooking.TotalPrice.Amount,
            savedBooking.TotalPrice.Currency.ToString());

        var domainEventHandler = new BookingCreatedDomainEventHandler(
            eventBusMock.Object,
            propertyRepository,
            loggerMock.Object);

        await domainEventHandler.Handle(domainEvent, CancellationToken.None);

        // Assert - Integration Event Publishing
        eventBusMock.Verify(eb => eb.PublishAsync(
            It.Is<BookingRequestedIntegrationEvent>(e => 
                e.BookingId == savedBooking.Id && 
                e.PropertyId == propertyId &&
                e.HostId == hostId &&
                e.RenterId == guestId), 
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
