using Triapka.Application.DTOs;

namespace Triapka.Application.Interfaces;

public interface IAuthService
{
    Task<(bool Success, IEnumerable<string> Errors, string? UserId, string? EmailToken)> RegisterAsync(RegisterDto dto);

    Task<bool> ConfirmEmailAsync(string userId, string token);

    Task<bool> LoginAsync(LoginDto dto);

    Task<(bool Success, string? Token)> ForgotPasswordAsync(ForgotPasswordDto dto);

    Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordDto dto);
}