using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RentIt.Modules.Properties.Application;

public static class PropertiesApplicationServiceRegistration
{
    public static IServiceCollection AddPropertiesApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
