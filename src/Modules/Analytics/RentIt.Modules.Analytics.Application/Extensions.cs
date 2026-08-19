using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RentIt.Modules.Analytics.Application;

public static class Extensions
{
    public static IServiceCollection AddAnalyticsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}
