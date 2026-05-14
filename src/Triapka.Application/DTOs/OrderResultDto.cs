namespace Triapka.Application.DTOs;

public class OrderResultDto
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = string.Empty;

    public int TotalItems { get; set; }
}
