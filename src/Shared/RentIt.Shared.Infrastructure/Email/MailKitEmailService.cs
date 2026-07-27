using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using RentIt.Shared.Abstractions.Email;

namespace RentIt.Shared.Infrastructure.Email;

internal sealed class MailKitEmailService(
    IConfiguration configuration,
    ILogger<MailKitEmailService> logger) : IEmailService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<MailKitEmailService> _logger = logger;

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        await SendEmailInternalAsync(to, subject, body, null, null, cancellationToken);
    }

    public async Task SendEmailWithHtmlAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        await SendEmailInternalAsync(to, subject, null, htmlBody, null, cancellationToken);
    }

    public async Task SendEmailWithAttachmentsAsync(string to, string subject, string? plainTextBody, string? htmlBody, List<EmailAttachment> attachments, CancellationToken cancellationToken = default)
    {
        await SendEmailInternalAsync(to, subject, plainTextBody, htmlBody, attachments, cancellationToken);
    }

    private async Task SendEmailInternalAsync(string to, string subject, string? plainTextBody, string? htmlBody, List<EmailAttachment>? attachments, CancellationToken cancellationToken)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var fromEmail = smtpSettings["FromEmail"] ?? "noreply@rentit.com";
        var fromName = smtpSettings["FromName"] ?? "RentIt";
        var host = smtpSettings["Host"] ?? "localhost";
        var portStr = smtpSettings["Port"] ?? "587";
        var port = int.TryParse(portStr, out var p) ? p : 587;
        var username = smtpSettings["Username"];
        var password = smtpSettings["Password"];

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var builder = new BodyBuilder();

        if (!string.IsNullOrEmpty(plainTextBody))
        {
            builder.TextBody = plainTextBody;
        }

        if (!string.IsNullOrEmpty(htmlBody))
        {
            builder.HtmlBody = htmlBody;
        }

        if (attachments != null && attachments.Any())
        {
            foreach (var attachment in attachments)
            {
                builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
            }
        }

        message.Body = builder.ToMessageBody();

        try
        {
            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls, cancellationToken);
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await smtpClient.AuthenticateAsync(username, password, cancellationToken);
            }
            
            await smtpClient.SendAsync(message, cancellationToken);
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToAddress}", to);
            throw new InvalidOperationException($"Failed to send email to {to}", ex);
        }
    }
}

