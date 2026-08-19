using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RentIt.Modules.Verification.Application;

public static class Extensions
{
    public static IServiceCollection AddVerificationApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
