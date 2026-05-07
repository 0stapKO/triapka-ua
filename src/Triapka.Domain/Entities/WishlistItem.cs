namespace Triapka.Domain.Entities;

public class WishlistItem
{
    public int WishlistItemId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public virtual ApplicationUser? User { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;
}