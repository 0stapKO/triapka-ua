using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Triapka.Application.Interfaces;

namespace Triapka.Infrastructure.Services;

public class MailKitEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<MailKitEmailService> _logger;

    public MailKitEmailService(IConfiguration config, ILogger<MailKitEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpSection = _config.GetSection("Smtp");

        var host = smtpSection["Host"] ?? throw new InvalidOperationException("Smtp:Host is not configured.");
        var port = int.Parse(smtpSection["Port"] ?? "587");
        var user = smtpSection["Username"] ?? throw new InvalidOperationException("Smtp:Username is not configured.");
        var pass = smtpSection["Password"] ?? throw new InvalidOperationException("Smtp:Password is not configured.");
        var fromName = smtpSection["FromName"] ?? "Triapka.ua";
        var fromEmail = smtpSection["FromEmail"] ?? user;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(user, pass);
            await client.SendAsync(message);
            _logger.LogInformation("Email sent to {To} with subject '{Subject}'", to, subject);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
