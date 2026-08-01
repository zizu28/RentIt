using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RentIt.Modules.Payments.Application;

public static class PaymentsApplicationServiceRegistration
{
    public static IServiceCollection AddPaymentsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }
}
