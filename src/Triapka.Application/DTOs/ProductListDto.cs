namespace Triapka.Application.DTOs;
public class ProductListDto
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string MainImageUrl { get; set; } = null!;

    public decimal? Rating { get; set; }
}
