using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Bookings.Domain.Repositories;
using RentIt.Modules.Bookings.Infrastructure.Database;
using RentIt.Modules.Bookings.Infrastructure.Repositories;


namespace RentIt.Modules.Bookings.Infrastructure;

public static class BookingsInfrastructureServiceRegistration
{
    public static IServiceCollection AddBookingsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookingsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("BookingsDatabase")));
            // Note: If you have an interceptor for PublishDomainEventsInterceptor, it should be registered here.
            
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookablePropertyRepository, BookablePropertyRepository>();

        return services;
    }
}
