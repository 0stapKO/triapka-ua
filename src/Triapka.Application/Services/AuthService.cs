using Microsoft.AspNetCore.Identity;

using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Application.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : IAuthService
{
    public async Task<(bool Success, IEnumerable<string> Errors, string? UserId, string? EmailToken)> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description), null, null);
        }

        // Автоматично призначаємо всім новим юзерам роль Customer
        await userManager.AddToRoleAsync(user, "Customer");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        return (true, [], user.Id, token);
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var result = await userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<bool> LoginAsync(LoginDto dto)
    {
        // false в кінці означає, що ми не блокуємо акаунт після невдалих спроб
        var result = await signInManager.PasswordSignInAsync(
            dto.Email, dto.Password, dto.RememberMe, lockoutOnFailure: false);

        return result.Succeeded;
    }
}