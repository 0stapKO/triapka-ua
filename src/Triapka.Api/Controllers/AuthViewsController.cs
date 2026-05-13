using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Triapka.Application.DTOs;
using Triapka.Domain.Entities;

namespace Triapka.Api.Controllers;

[Route("Auth")]
public class AuthViewsController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthViewsController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet("Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View("~/Views/Auth/Login.cshtml");
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return Redirect("/");
            }

            ModelState.AddModelError(string.Empty, "Невірний логін або пароль.");
        }

        return View("~/Views/Auth/Login.cshtml", dto);
    }

    [HttpGet("Register")]
    public IActionResult Register()
    {
        return View("~/Views/Auth/Register.cshtml");
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Redirect("/");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View("~/Views/Auth/Register.cshtml", dto);
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }

    [HttpGet("ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        return View("~/Views/Auth/ForgotPassword.cshtml");
    }

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromServices] Triapka.Application.Interfaces.IEmailService emailService, ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Auth/ForgotPassword.cshtml", dto);
        }

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            // Не розкриваємо, що користувача не знайдено
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action(
            "ResetPassword",
            "AuthViews",
            new { email = user.Email, code = code },
            protocol: Request.Scheme);

        await emailService.SendEmailAsync(
            dto.Email,
            "Скидання пароля - Triapka.ua",
            $"Для скидання пароля перейдіть за посиланням: <a href='{callbackUrl}'>Натисніть тут</a>");

        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    [HttpGet("ForgotPasswordConfirmation")]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View("~/Views/Auth/ForgotPasswordConfirmation.cshtml");
    }

    [HttpGet("ResetPassword")]
    public IActionResult ResetPassword(string? code = null, string? email = null)
    {
        if (code == null)
        {
            return BadRequest("Для скидання пароля необхідний код.");
        }

        ResetPasswordDto model = new ResetPasswordDto { Token = code, Email = email ?? string.Empty };
        return View("~/Views/Auth/ResetPassword.cshtml", model);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Auth/ResetPassword.cshtml", dto);
        }

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            // Не розкриваємо, що користувача не знайдено
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View("~/Views/Auth/ResetPassword.cshtml", dto);
    }

    [HttpGet("ResetPasswordConfirmation")]
    public IActionResult ResetPasswordConfirmation()
    {
        return View("~/Views/Auth/ResetPasswordConfirmation.cshtml");
    }
}