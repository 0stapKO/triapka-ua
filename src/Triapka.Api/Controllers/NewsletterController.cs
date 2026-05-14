using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;

namespace Triapka.Api.Controllers;

/// <summary>
/// Адміністративний контролер для керування розсилкою.
/// Усі дії доступні лише ролі Admin.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("Admin/Newsletter")]
public class NewsletterController : Controller
{
    private readonly INewsletterService _newsletterService;
    private readonly ILogger<NewsletterController> _logger;

    public NewsletterController(
        INewsletterService newsletterService,
        ILogger<NewsletterController> logger)
    {
        _newsletterService = newsletterService;
        _logger = logger;
    }

    /// <summary>
    /// Показує форму для надсилання розсилки + кількість підписників.
    /// GET /Admin/Newsletter
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var emails = await _newsletterService.GetSubscribedEmailsAsync();
        ViewBag.SubscriberCount = emails.Count;
        return View("~/Views/Admin/Newsletter/Index.cshtml", new SendNewsletterDto());
    }

    /// <summary>
    /// Надсилає розсилку всім підписаним користувачам.
    /// POST /Admin/Newsletter
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(SendNewsletterDto dto)
    {
        var emails = await _newsletterService.GetSubscribedEmailsAsync();
        ViewBag.SubscriberCount = emails.Count;

        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/Newsletter/Index.cshtml", dto);
        }

        if (emails.Count == 0)
        {
            TempData["WarningMessage"] = "Немає підписників. Розсилку не надіслано.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await _newsletterService.SendNewsletterAsync(dto.Subject, dto.HtmlBody);
            _logger.LogInformation(
                "Newsletter '{Subject}' sent to {Count} subscribers by {User}.",
                dto.Subject,
                emails.Count,
                User.Identity?.Name);

            TempData["SuccessMessage"] = $"Розсилку надіслано {emails.Count} підписникам!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send newsletter '{Subject}'.", dto.Subject);
            TempData["ErrorMessage"] = $"Помилка при надсиланні: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Повертає JSON-список email активних підписників.
    /// GET /Admin/Newsletter/Subscribers
    /// </summary>
    [HttpGet("Subscribers")]
    public async Task<IActionResult> Subscribers()
    {
        var emails = await _newsletterService.GetSubscribedEmailsAsync();
        return Ok(new { count = emails.Count, emails });
    }
}
