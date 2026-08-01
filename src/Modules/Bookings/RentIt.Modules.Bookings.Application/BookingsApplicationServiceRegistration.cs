using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RentIt.Modules.Bookings.Application;

public static class BookingsApplicationServiceRegistration
{
    public static IServiceCollection AddBookingsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }
}
