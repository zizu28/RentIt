using Microsoft.Extensions.Logging;
using RentIt.Modules.Messaging.Application.Services;

namespace RentIt.Modules.Messaging.Infrastructure.Services;

internal sealed class MockSmsService(ILogger<MockSmsService> logger) : ISmsService
{
    private readonly ILogger<MockSmsService> _logger = logger;

    public Task SendSmsAsync(string to, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending SMS to {To}: {Message}", to, message);
        return Task.CompletedTask;
    }
}
