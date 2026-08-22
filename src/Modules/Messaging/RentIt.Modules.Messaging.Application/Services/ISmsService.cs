namespace RentIt.Modules.Messaging.Application.Services;

public interface ISmsService
{
    Task SendSmsAsync(string to, string message, CancellationToken cancellationToken = default);
}
