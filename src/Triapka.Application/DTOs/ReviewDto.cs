namespace Triapka.Application.DTOs;

public class ReviewDto
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
