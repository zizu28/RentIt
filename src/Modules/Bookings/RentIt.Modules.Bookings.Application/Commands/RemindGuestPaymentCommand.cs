using MediatR;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Abstractions.Email;
using RentIt.Shared.Contracts.Identity.Queries;

namespace RentIt.Modules.Bookings.Application.Commands;

public record RemindGuestPaymentCommand(Guid BookingId, Guid HostId) : IRequest;

public class RemindGuestPaymentCommandHandler(
    IBookingRepository bookingRepository,
    IBookablePropertyRepository propertyRepository,
    Serilog.ILogger logger,
    IBackgroundJob backgroundJob,
    ISender sender) : IRequestHandler<RemindGuestPaymentCommand>
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookablePropertyRepository _propertyRepository = propertyRepository;
    private readonly Serilog.ILogger _logger = logger;
    private readonly IBackgroundJob _backgroundJob = backgroundJob;
    private readonly ISender _sender = sender;

    public async Task Handle(RemindGuestPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Attempting to send payment reminder for Booking {BookingId} by Host {HostId}", request.BookingId, request.HostId);

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            _logger.Warning("Booking {BookingId} not found.", request.BookingId);
            return;
        }

        var property = await _propertyRepository.GetByIdAsync(booking.PropertyId, cancellationToken);
        if (property == null || property.HostId != request.HostId)
        {
            _logger.Warning("Host {HostId} is not authorized to send reminders for booking {BookingId}.", request.HostId, request.BookingId);
            return;
        }

        if (booking.Status != BookingStatus.Pending)
        {
            _logger.Warning("Booking {BookingId} is not in Pending status. Current status: {Status}", request.BookingId, booking.Status);
            return;
        }

        string? guestEmail = null;
        var result = await _sender.Send(new GetUserEmailQuery(booking.GuestId), cancellationToken);
        
        if (result.IsSuccess)
        {
            guestEmail = result.Value;
        }

        if (!string.IsNullOrEmpty(guestEmail))
        {
            string body = $"Hello! This is a reminder to complete your payment for your upcoming trip to '{property.Title}' (Booking Ref: {booking.Id}). Please log in to your account and go to your Pending Payments page to complete the transaction.";
            
            _backgroundJob.Enqueue<IEmailService>("alpha", emailService => emailService.SendEmailAsync(guestEmail, $"Payment Reminder for {property.Title}", body, CancellationToken.None));
            
            _logger.Information("Successfully enqueued payment reminder email for Booking {BookingId} to {GuestEmail}", request.BookingId, guestEmail);
        }
        else
        {
            _logger.Warning("Could not find email address for Guest {GuestId} in booking {BookingId}", booking.GuestId, booking.Id);
        }
    }
}
