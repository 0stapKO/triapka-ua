using System.ComponentModel.DataAnnotations;

namespace Triapka.Application.DTOs;

public class SendNewsletterDto
{
    [Required(ErrorMessage = "Тема листа обов'язкова")]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Тіло листа обов'язкове")]
    public string HtmlBody { get; set; } = string.Empty;
}
