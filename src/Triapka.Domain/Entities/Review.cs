namespace Triapka.Domain.Entities;

public class Review
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Product Product { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;
}
