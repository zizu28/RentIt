using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Identity.Application.Commands;

namespace RentIt.Modules.Identity.Application;

public static class IdentityApplicationServiceRegistration
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(LoginUserCommand).Assembly);
        });

        return services;
    }
}
