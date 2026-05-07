using Microsoft.Extensions.Logging;
using Triapka.Application.Interfaces;

namespace Triapka.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        // В реальному проекті тут буде логіка відправки через SMTP (MailKit) або API (SendGrid)
        _logger.LogWarning("MOCK EMAIL SENT TO: {To}", to);
        _logger.LogWarning("SUBJECT: {Subject}", subject);
        _logger.LogWarning("BODY: {Body}", body);

        return Task.CompletedTask;
    }
}
