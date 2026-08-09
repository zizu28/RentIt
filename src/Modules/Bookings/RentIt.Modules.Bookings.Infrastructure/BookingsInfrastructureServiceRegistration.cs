using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Bookings.Application.Services;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Modules.Bookings.Infrastructure.BackgroundJobs;
using RentIt.Modules.Bookings.Infrastructure.Database;
using RentIt.Modules.Bookings.Infrastructure.Repositories;
using RentIt.Modules.Bookings.Infrastructure.Services;
using RentIt.Shared.Abstractions.Persistence;
using Hangfire;

namespace RentIt.Modules.Bookings.Infrastructure;

public static class BookingsInfrastructureServiceRegistration
{
    public static IServiceCollection AddBookingsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookingsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("BookingsDatabase")));
            
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookablePropertyRepository, BookablePropertyRepository>();
        
        services.AddScoped<DomainEventDispatcher>();
        services.AddKeyedScoped<IUnitOfWork, BookingsUnitOfWork>("Bookings");
        services.AddScoped<IBookingsOutboxService, BookingsOutboxService>();
        services.AddScoped<IBookingsInboxService, BookingsInboxService>();

        // Register the background job logic for Hangfire to inject dependencies
        services.AddScoped<PendingBookingReminderJob>();

        return services;
    }

    public static void ConfigureBookingsJobs()
    {
        RecurringJob.AddOrUpdate<PendingBookingReminderJob>(
            "pending-booking-reminders", 
            job => job.ProcessPendingBookingsAsync(CancellationToken.None), 
            Cron.HourInterval(12));
    }
}
