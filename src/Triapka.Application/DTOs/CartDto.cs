namespace Triapka.Application.DTOs;
public class CartDto
{
    public int CartId { get; set; }

    public List<CartItemDto> CartItems { get; set; } = [];

    public decimal TotalAmount { get; set; }
}