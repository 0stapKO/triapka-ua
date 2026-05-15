namespace Triapka.Application.Interfaces;

using Triapka.Domain.Entities;
public interface IReviewRepository
{
    Task AddReviewAsync(Review review);

    Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);

    Task<Review?> GetReviewByIdAsync(int reviewId);

    Task UpdateReviewAsync(Review review);

    Task DeleteReviewAsync(int reviewId);
}
