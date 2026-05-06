using Microsoft.AspNetCore.Mvc;

using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;

namespace Triapka.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    IEmailService emailService,
    ILogger<AuthController> logger,
    LinkGenerator linkGenerator,
    IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    /// <summary>
    /// Реєстрація нового користувача.
    /// Після успішної реєстрації надсилається лист з посиланням для підтвердження email.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errors, userId, emailToken) = await authService.RegisterAsync(dto);

        if (!success)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return BadRequest(ModelState);
        }

        // Генеруємо посилання для підтвердження email
        var confirmationLink = linkGenerator.GetUriByAction(
            httpContextAccessor.HttpContext!,
            action: nameof(ConfirmEmail),
            controller: "Auth",
            values: new { userId, token = emailToken });

        logger.LogInformation(
            "Посилання для підтвердження email для {Email}: {Link}",
            dto.Email,
            confirmationLink);

        await emailService.SendEmailAsync(
            dto.Email,
            "Підтвердження email — Triapka",
            $"Будь ласка, підтвердіть вашу електронну адресу, перейшовши за посиланням:\n{confirmationLink}");

        return Ok(new
        {
            message = "Реєстрація успішна. Перевірте вашу пошту для підтвердження акаунту.",
            userId,
        });
    }

    /// <summary>
    /// Підтвердження email за токеном із листа.
    /// </summary>
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new { message = "Невірні параметри підтвердження." });
        }

        var success = await authService.ConfirmEmailAsync(userId, token);

        if (!success)
        {
            return BadRequest(new { message = "Не вдалося підтвердити email. Посилання недійсне або застаріле." });
        }

        return Ok(new { message = "Email успішно підтверджено. Тепер ви можете увійти." });
    }

    /// <summary>
    /// Логін користувача. Встановлює cookie сесії.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await authService.LoginAsync(dto);

        if (!success)
        {
            return Unauthorized(new { message = "Невірний email або пароль." });
        }

        return Ok(new { message = "Вхід успішний." });
    }
}
