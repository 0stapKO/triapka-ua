using System.ComponentModel.DataAnnotations;

namespace Triapka.Application.DTOs;

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
}