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
    /// <param name="dto">The registration data transfer object containing user details.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (dto is null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

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
    /// <param name="userId">The unique identifier of the user whose email is being confirmed.</param>
    /// <param name="token">The token sent to the user's email for confirmation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
    /// <param name="dto">The registration data transfer object containing user details.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Запит на відновлення пароля.
    /// </summary>
    /// <param name="dto">The registration data transfer object containing user details.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, token) = await authService.ForgotPasswordAsync(dto);

        // Always return 200 OK for security reasons to avoid revealing if the email exists
        if (!success || string.IsNullOrEmpty(token))
        {
            return Ok(new { message = "Якщо вказаний email існує, ми надіслали інструкції з відновлення пароля." });
        }

        var resetLink = linkGenerator.GetUriByAction(
            httpContextAccessor.HttpContext!,
            action: nameof(ResetPassword),
            controller: "Auth",
            values: new { email = dto.Email, token = token });

        logger.LogInformation(
            "Посилання для відновлення пароля для {Email}: {Link}",
            dto.Email,
            resetLink);

        await emailService.SendEmailAsync(
            dto.Email,
            "Відновлення пароля — Triapka",
            $"Для відновлення пароля перейдіть за посиланням:\n{resetLink}");

        return Ok(new { message = "Якщо вказаний email існує, ми надіслали інструкції з відновлення пароля." });
    }

    /// <summary>
    /// Скидання пароля за токеном.
    /// </summary>
    /// <param name="dto">The registration data transfer object containing user details.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errors) = await authService.ResetPasswordAsync(dto);

        if (!success)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return BadRequest(ModelState);
        }

        return Ok(new { message = "Пароль успішно змінено." });
    }
}
