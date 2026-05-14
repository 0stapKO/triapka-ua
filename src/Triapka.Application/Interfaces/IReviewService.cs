namespace Triapka.Application.Interfaces;

using Triapka.Application.DTOs;

public interface IReviewService
{
    Task AddReviewAsync(CreateReview review);

    Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId);

    Task<ReviewDto?> GetReviewByIdAsync(int reviewId);

    Task UpdateReviewAsync(ReviewDto review);

    Task DeleteReviewAsync(int reviewId);
}
