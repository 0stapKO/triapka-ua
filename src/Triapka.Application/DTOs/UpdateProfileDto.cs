using System.ComponentModel.DataAnnotations;

namespace Triapka.Application.DTOs;

public class UpdateProfileDto
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [Phone(ErrorMessage = "Невірний формат номеру телефону")]
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
}
