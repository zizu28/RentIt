namespace RentIt.Shared.Abstractions.Email;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendEmailWithHtmlAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
