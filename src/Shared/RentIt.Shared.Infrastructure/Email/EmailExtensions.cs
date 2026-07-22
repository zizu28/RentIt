using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentIt.Shared.Abstractions.Email;

namespace RentIt.Shared.Infrastructure.Email;

public static class EmailExtensions
{
    public static IServiceCollection AddSharedEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        var smtpSettings = configuration.GetSection("SmtpSettings");
        
        var fromEmail = smtpSettings["FromEmail"] ?? "noreply@rentit.com";
        var fromName = smtpSettings["FromName"] ?? "RentIt";
        var host = smtpSettings["Host"] ?? "localhost";
        var portStr = smtpSettings["Port"] ?? "25";
        var port = int.TryParse(portStr, out var p) ? p : 25;

        services.AddFluentEmail(fromEmail, fromName)
                .AddSmtpSender(host, port);

        services.AddScoped<IEmailService, FluentEmailService>();

        return services;
    }
}
