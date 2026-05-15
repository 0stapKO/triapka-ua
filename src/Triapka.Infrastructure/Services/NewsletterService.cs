using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Infrastructure.Services;

public class NewsletterService : INewsletterService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<NewsletterService> _logger;

    public NewsletterService(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILogger<NewsletterService> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetSubscribedEmailsAsync()
    {
        var subscribers = await _userManager.Users
            .Where(u => u.IsSubscribedToNewsletter && u.Email != null)
            .Select(u => u.Email!)
            .ToListAsync();

        return subscribers;
    }

    /// <inheritdoc />
    public async Task SendNewsletterAsync(string subject, string htmlBody)
    {
        var emails = await GetSubscribedEmailsAsync();

        if (emails.Count == 0)
        {
            _logger.LogInformation("Newsletter send skipped — no active subscribers.");
            return;
        }

        _logger.LogInformation("Sending newsletter '{Subject}' to {Count} subscriber(s).", subject, emails.Count);

        var tasks = emails.Select(email =>
            _emailService.SendEmailAsync(email, subject, htmlBody));

        await Task.WhenAll(tasks);

        _logger.LogInformation("Newsletter '{Subject}' sent successfully.", subject);
    }
}
