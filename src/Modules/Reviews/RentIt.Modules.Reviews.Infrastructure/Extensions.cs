using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Reviews.Domain.Repositories;
using RentIt.Modules.Reviews.Infrastructure.Database;
using RentIt.Modules.Reviews.Infrastructure.Repositories;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Reviews.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddReviewsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReviewsDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("ReviewsDatabase"));
        });

        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddKeyedScoped<IUnitOfWork, ReviewsUnitOfWork>("Reviews");

        return services;
    }
}
