using Microsoft.Extensions.Logging;

using Triapka.Application.Interfaces;

namespace Triapka.Infrastructure.Services;

public class ConsoleEmailService(ILogger<ConsoleEmailService> logger) : IEmailService
{
    public Task SendEmailAsync(string toEmail, string subject, string message)
    {
        logger.LogInformation("=== ІМІТАЦІЯ ВІДПРАВКИ EMAIL ===");
        logger.LogInformation("Кому: {ToEmail}", toEmail);
        logger.LogInformation("Тема: {Subject}", subject);
        logger.LogInformation("Текст:\n{Message}", message);
        logger.LogInformation("================================");

        return Task.CompletedTask;
    }
}