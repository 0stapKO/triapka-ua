namespace Triapka.Application.DTOs;
public class CartItemDto
{
    public int ItemId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TotalPrice => Quantity * Price;
}
