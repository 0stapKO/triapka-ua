using System.ComponentModel.DataAnnotations;

namespace Triapka.Application.DTOs;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Введіть поточний пароль")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть новий пароль")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Мінімум 8 символів")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Підтвердіть новий пароль")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Паролі не збігаються")]
    public string ConfirmPassword { get; set; } = string.Empty;
}