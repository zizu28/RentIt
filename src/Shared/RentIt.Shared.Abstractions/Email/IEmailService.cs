namespace RentIt.Shared.Abstractions.Email;

public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "application/octet-stream";
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendEmailWithHtmlAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
    Task SendEmailWithAttachmentsAsync(string to, string subject, string? plainTextBody, string? htmlBody, List<EmailAttachment> attachments, CancellationToken cancellationToken = default);
}
