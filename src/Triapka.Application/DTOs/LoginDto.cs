using System.ComponentModel.DataAnnotations;

namespace Triapka.Application.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Email є обов'язковим")]
    [EmailAddress(ErrorMessage = "Невірний формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль є обов'язковим")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}