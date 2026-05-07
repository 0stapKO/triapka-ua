namespace Triapka.Domain.Entities;

public class Cart
{
    public int CartId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public virtual ApplicationUser? User { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = [];
}