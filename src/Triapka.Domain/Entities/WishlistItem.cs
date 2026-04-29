namespace Triapka.Domain.Entities;

public class WishlistItem
{
    public int WishlistItemId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;
}
