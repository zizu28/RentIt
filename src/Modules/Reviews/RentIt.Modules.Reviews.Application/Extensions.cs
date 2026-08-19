using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RentIt.Modules.Reviews.Application;

public static class Extensions
{
    public static IServiceCollection AddReviewsApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
