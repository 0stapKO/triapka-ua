namespace Triapka.Domain.Entities;

public class ProductCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = [];
}
