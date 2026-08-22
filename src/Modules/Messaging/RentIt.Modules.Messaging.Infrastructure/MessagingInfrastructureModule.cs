using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Modules.Messaging.Application.Services;
using RentIt.Modules.Messaging.Domain.Repositories;
using RentIt.Modules.Messaging.Infrastructure.Persistence;
using RentIt.Modules.Messaging.Infrastructure.Persistence.Repositories;
using RentIt.Modules.Messaging.Infrastructure.Services;
using RentIt.Shared.Abstractions.Persistence;

namespace RentIt.Modules.Messaging.Infrastructure;

public static class MessagingInfrastructureModule
{
    public static IServiceCollection AddMessagingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MessagingDatabase");

        services.AddDbContext<MessagingDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IMessagingUserRepository, MessagingUserRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MessagingDbContext>());

        services.AddTransient<IEmailService, MockEmailService>();
        services.AddTransient<ISmsService, MockSmsService>();

        return services;
    }
}
