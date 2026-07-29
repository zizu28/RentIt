using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Properties.Domain.Repositories;
using RentIt.Modules.Properties.Infrastructure.Database;
using RentIt.Modules.Properties.Infrastructure.Repositories;

namespace RentIt.Modules.Properties.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddPropertiesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PropertiesDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("PropertiesDatabase"));
        });

        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<RentIt.Modules.Properties.Application.Services.IPropertyEmailService, RentIt.Modules.Properties.Infrastructure.Services.PropertyEmailService>();

        return services;
    }
}
