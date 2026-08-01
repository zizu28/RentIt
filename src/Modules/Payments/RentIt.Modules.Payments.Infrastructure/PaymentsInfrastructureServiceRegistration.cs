using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Payments.Application.Services;
using RentIt.Modules.Payments.Domain.Repositories;
using RentIt.Modules.Payments.Infrastructure.Database;
using RentIt.Modules.Payments.Infrastructure.Database.Repositories;
using RentIt.Modules.Payments.Infrastructure.Services;

namespace RentIt.Modules.Payments.Infrastructure;

public static class PaymentsInfrastructureServiceRegistration
{
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentsUnitOfWork, PaymentsUnitOfWork>();
        
        services.AddHttpClient<IPaystackService, PaystackService>();

        return services;
    }
}
