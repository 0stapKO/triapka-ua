namespace Triapka.Application.DTOs;

public class OrderItemDto
{
    public int OrderItemId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int Quantity { get; set; }

    public decimal PriceAtPurchase { get; set; }

    public decimal LineTotal => Quantity * PriceAtPurchase;
}

public class OrderDto
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public List<OrderItemDto> Items { get; set; } = [];

    /// <summary>Gets localised Ukrainian status label.</summary>
    public string StatusLabel => Status switch
    {
        "Pending" => "В обробці",
        "Confirmed" => "Підтверджено",
        "Shipped" => "Відправлено",
        "Delivered" => "Доставлено",
        "Cancelled" => "Скасовано",
        _ => Status
    };

    /// <summary>Gets cSS class suffix for the status badge.</summary>
    public string StatusCssClass => Status switch
    {
        "Pending" => "pending",
        "Confirmed" => "confirmed",
        "Shipped" => "shipped",
        "Delivered" => "delivered",
        "Cancelled" => "cancelled",
        _ => "pending"
    };
}
