using Microsoft.Extensions.Logging;
using RentIt.Modules.Messaging.Application.Services;

namespace RentIt.Modules.Messaging.Infrastructure.Services;

internal sealed class MockEmailService(ILogger<MockEmailService> logger) : IEmailService
{
    private readonly ILogger<MockEmailService> _logger = logger;

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending Email to {To}: {Subject} - {Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
