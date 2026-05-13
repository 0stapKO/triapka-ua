using System.ComponentModel.DataAnnotations;

namespace Triapka.Application.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Email є обов'язковим")]
    [EmailAddress(ErrorMessage = "Невірний формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль є обов'язковим")]
    [MinLength(8, ErrorMessage = "Пароль має містити мінімум 8 символів")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Підтвердження пароля є обов'язковим")]
    [Compare("Password", ErrorMessage = "Паролі не співпадають")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}