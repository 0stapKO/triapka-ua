namespace Triapka.Domain.Entities;

public class Order
{
    public int OrderId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public virtual ApplicationUser? User { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
