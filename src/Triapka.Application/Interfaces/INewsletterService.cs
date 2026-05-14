namespace Triapka.Application.Interfaces;

public interface INewsletterService
{
    /// <summary>
    /// Returns email addresses of all users who opted in to the newsletter.
    /// </summary>
    Task<IReadOnlyList<string>> GetSubscribedEmailsAsync();

    /// <summary>
    /// Sends <paramref name="subject"/>/<paramref name="htmlBody"/> to every subscribed user.
    /// </summary>
    Task SendNewsletterAsync(string subject, string htmlBody);
}
