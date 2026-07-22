using FluentEmail.Core;
using RentIt.Shared.Abstractions.Email;

namespace RentIt.Shared.Infrastructure.Email;

internal sealed class FluentEmailService(IFluentEmailFactory fluentEmailFactory) : IEmailService
{
    private readonly IFluentEmailFactory _fluentEmailFactory = fluentEmailFactory;

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var email = _fluentEmailFactory.Create();
        
        var response = await email
            .To(to)
            .Subject(subject)
            .Body(body, isHtml: false)
            .SendAsync(cancellationToken);

        if (!response.Successful)
        {
            // Ideally use an ILogger to log error messages here
            var errors = string.Join(", ", response.ErrorMessages);
            throw new InvalidOperationException($"Failed to send email to {to}: {errors}");
        }
    }

    public async Task SendEmailWithHtmlAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var email = _fluentEmailFactory.Create();
        
        var response = await email
            .To(to)
            .Subject(subject)
            .Body(htmlBody, isHtml: true)
            .SendAsync(cancellationToken);

        if (!response.Successful)
        {
            var errors = string.Join(", ", response.ErrorMessages);
            throw new InvalidOperationException($"Failed to send html email to {to}: {errors}");
        }
    }
}
