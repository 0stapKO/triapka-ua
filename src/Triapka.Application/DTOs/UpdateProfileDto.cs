using System.ComponentModel.DataAnnotations;

namespace Triapka.Application.DTOs;

public class UpdateProfileDto
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [Required(ErrorMessage = "Телефон є обов'язковим")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Введіть коректний номер у форматі +380XXXXXXXXX")]
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
}