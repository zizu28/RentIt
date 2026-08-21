using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Analytics.Domain.Repositories;
using RentIt.Modules.Analytics.Infrastructure.Database;
using RentIt.Modules.Analytics.Infrastructure.Repositories;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Analytics.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddAnalyticsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AnalyticsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AnalyticsDatabase")));

        services.AddScoped<IPropertyMetricsRepository, PropertyMetricsRepository>();
        services.AddScoped<IHostMetricsRepository, HostMetricsRepository>();

        services.AddKeyedScoped<IUnitOfWork, AnalyticsUnitOfWork>("Analytics");

        return services;
    }
}
