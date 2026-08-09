using Hangfire;
using Microsoft.EntityFrameworkCore;
using MediatR;
using RentIt.Modules.Bookings.Domain.Enums;
using RentIt.Modules.Bookings.Infrastructure.Database;
using RentIt.Shared.Abstractions.BackgroundJobs;
using RentIt.Shared.Abstractions.Email;
using RentIt.Shared.Contracts.Identity.Queries;

namespace RentIt.Modules.Bookings.Infrastructure.BackgroundJobs;

public class PendingBookingReminderJob(
    BookingsDbContext bookingsContext,
    Serilog.ILogger logger,
    IBackgroundJob backgroundJob,
    ISender sender)
{
    private readonly BookingsDbContext _bookingsContext = bookingsContext;
    private readonly Serilog.ILogger _logger = logger;
    private readonly IBackgroundJob _backgroundJob = backgroundJob;
    private readonly ISender _sender = sender;

    public async Task ProcessPendingBookingsAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Processing pending bookings for email reminders via Hangfire...");

        // Find bookings that have been pending.
        var pendingBookings = await _bookingsContext.Bookings
            .Where(b => b.Status == BookingStatus.Pending)
            .ToListAsync(cancellationToken);

        foreach (var booking in pendingBookings)
        {
            string? guestEmail = null;
            
            // Cross-module query to fetch the Guest's email from the Identity module via MediatR
            var result = await _sender.Send(new GetUserEmailQuery(booking.GuestId), cancellationToken);
            
            if (result.IsSuccess)
            {
                guestEmail = result.Value;
            }

            if (!string.IsNullOrEmpty(guestEmail))
            {
                string body = $"Reminder to Guest {booking.GuestId} to complete payment for Booking {booking.Id}. Property: {booking.PropertyId}";
                _backgroundJob.Enqueue<IEmailService>("alpha", emailService => emailService.SendEmailAsync(guestEmail, "PENDING PAYMENT REMINDER", body, CancellationToken.None));
            }
            else
            {
                _logger.Warning("Could not find email address for Guest {GuestId} in booking {BookingId}", booking.GuestId, booking.Id);
            }
        }
    }
}
