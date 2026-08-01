using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Bookings.Application;
using RentIt.Modules.Bookings.Infrastructure;

namespace RentIt.Modules.Bookings.Api;

public static class BookingsModuleExtensions
{
    public static IServiceCollection AddBookingsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBookingsApplication();
        services.AddBookingsInfrastructure(configuration);

        return services;
    }
}
