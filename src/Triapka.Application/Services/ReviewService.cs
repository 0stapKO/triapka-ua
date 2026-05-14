namespace Triapka.Application.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;
using Triapka.Application.DTOs;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository reviewRepository;
    private readonly IProductRepository productRepository;

    public ReviewService(IReviewRepository reviewRepository, IProductRepository productRepository)
    {
        this.reviewRepository = reviewRepository;
        this.productRepository = productRepository;
    }

    public async Task AddReviewAsync(CreateReview dto)
    {
        var review = new Review
        {
            ProductId = dto.ProductId,
            UserId = dto.UserId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await reviewRepository.AddReviewAsync(review);

        await UpdateProductRatingAsync(dto.ProductId);
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId)
    {
        var reviews = await reviewRepository.GetReviewsByProductIdAsync(productId);

        return reviews.Select(r => new ReviewDto
        {
            ReviewId = r.ReviewId,
            ProductId = r.ProductId,
            UserId = r.UserId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<ReviewDto?> GetReviewByIdAsync(int reviewId)
    {
        var review = await reviewRepository.GetReviewByIdAsync(reviewId);

        if (review == null)
        {
            return null;
        }

        return new ReviewDto
        {
            ReviewId = review.ReviewId,
            ProductId = review.ProductId,
            UserId = review.UserId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }

    public async Task UpdateReviewAsync(ReviewDto dto)
    {
        var review = await reviewRepository.GetReviewByIdAsync(dto.ReviewId);

        if (review != null)
        {
            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            await reviewRepository.UpdateReviewAsync(review);

            await UpdateProductRatingAsync(review.ProductId);
        }
    }

    public async Task DeleteReviewAsync(int reviewId)
    {
        var review = await reviewRepository.GetReviewByIdAsync(reviewId);

        if (review != null)
        {
            var productId = review.ProductId;

            await reviewRepository.DeleteReviewAsync(reviewId);

            await UpdateProductRatingAsync(productId);
        }
    }

    private async Task UpdateProductRatingAsync(int productId)
    {
        var allReviews = await reviewRepository.GetReviewsByProductIdAsync(productId);
        var product = await productRepository.GetByIdAsync(productId);

        if (product != null)
        {
            if (allReviews.Any())
            {
                product.Rating = (decimal)allReviews.Average(r => r.Rating);
            }
            else
            {
                product.Rating = 0;
            }

            await productRepository.UpdateProductAsync(product);
        }
    }
}