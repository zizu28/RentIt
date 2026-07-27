using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Email;

namespace RentIt.Shared.Infrastructure.Email;

public static class EmailExtensions
{
    public static IServiceCollection AddSharedEmailServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, MailKitEmailService>();
        services.AddScoped<ITemplateService, FluidTemplateService>();

        return services;
    }
}
