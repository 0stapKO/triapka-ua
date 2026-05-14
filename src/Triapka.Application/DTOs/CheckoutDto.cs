using System.ComponentModel.DataAnnotations;

namespace Triapka.Application.DTOs;

public class CheckoutDto
{
    [Required(ErrorMessage = "Введіть адресу доставки.")]
    [MaxLength(300, ErrorMessage = "Адреса доставки не може бути довшою за 300 символів.")]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть номер телефону.")]
    [MaxLength(30, ErrorMessage = "Номер телефону не може бути довшим за 30 символів.")]
    public string Phone { get; set; } = string.Empty;
}
