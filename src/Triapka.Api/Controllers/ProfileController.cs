using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Triapka.Application.DTOs;
using Triapka.Domain.Entities;

namespace Triapka.Api.Controllers;

[Authorize]
[Route("Profile")]
public class ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : Controller
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        return user == null ? Redirect("/Auth/Login") : View("~/Views/Profile/Index.cshtml", user);
    }

    [HttpGet("Edit")]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Redirect("/Auth/Login");
        }

        var dto = new Triapka.Application.DTOs.UpdateProfileDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
        };

        return View("~/Views/Profile/Edit.cshtml", dto);
    }

    [HttpPost("Edit")]
    public async Task<IActionResult> Edit(Triapka.Application.DTOs.UpdateProfileDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Redirect("/Auth/Login");
        }

        if (ModelState.IsValid)
        {
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View("~/Views/Profile/Edit.cshtml", dto);
    }

    [HttpGet("Orders")]
    public IActionResult Orders()
    {
        return View("~/Views/Profile/Orders.cshtml");
    }

    [HttpGet("Settings")]
    public IActionResult Settings()
    {
        return View("~/Views/Profile/Settings.cshtml");
    }

    [HttpGet("ChangePassword")]
    public IActionResult ChangePassword()
    {
        return View("~/Views/Profile/ChangePassword.cshtml");
    }

    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Profile/ChangePassword.cshtml", dto);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction(nameof(Settings)); // Redirect back to settings on success
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View("~/Views/Profile/ChangePassword.cshtml", dto);
    }
}