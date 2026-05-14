namespace Triapka.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

public class ReviewRepository(ApplicationDbContext context) : IReviewRepository
{
    public async Task AddReviewAsync(Review review)
    {
        context.Reviews.Add(review);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
    {
        return await context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .ToListAsync();
    }

    public async Task<Review?> GetReviewByIdAsync(int reviewId)
    {
        return await context.Reviews.FindAsync(reviewId);
    }

    public async Task UpdateReviewAsync(Review review)
    {
        context.Reviews.Update(review);
        await context.SaveChangesAsync();
    }

    public async Task DeleteReviewAsync(int reviewId)
    {
        var review = await context.Reviews.FindAsync(reviewId);
        if (review != null)
        {
            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
        }
    }
}
