namespace Triapka.Application.DTOs;
public class ProductDetailsDto
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public List<string> ImageUrls { get; set; } = [];

    public string CategoryName { get; set; } = null!;

    public decimal? Rating { get; set; }
}
