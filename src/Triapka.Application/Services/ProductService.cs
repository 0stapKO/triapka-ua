using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Application.Services;

public class ProductService(IProductRepository productRepository, IReviewRepository reviewRepository) : IProductService
{
    public async Task<IEnumerable<ProductListDto>> GetAllProductsAsync()
    {
        var products = await productRepository.GetAllAsync();
        return products.Select(MapToProductListDto);
    }

    public async Task<ProductDetailsDto?> GetProductByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return null;
        }

        var dto = MapToProductDetailsDto(product);

        // Отримання відгуків
        var reviews = await reviewRepository.GetReviewsByProductIdAsync(id);
        dto.Reviews = reviews.Select(r =>
        {
            string displayName = "Користувач";

            if (r.User != null)
            {
                if (!string.IsNullOrWhiteSpace(r.User.FirstName))
                {
                    displayName = $"{r.User.FirstName} {r.User.LastName}".Trim();
                }
                else if (!string.IsNullOrEmpty(r.User.UserName))
                {
                    displayName = r.User.UserName.Split('@')[0];
                }
            }

            return new ReviewDto
            {
                ReviewId = r.ReviewId,
                ProductId = r.ProductId,
                UserId = r.UserId,
                UserName = displayName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            };
        }).ToList();

        // Отримання схожих товарів (4 останні з тієї ж категорії, окрім поточного)
        if (dto.CategoryId > 0)
        {
            var related = await GetRelatedProductsAsync(dto.CategoryId, id);
            dto.RelatedProducts = related.ToList();
        }

        return dto;
    }

    public async Task<IEnumerable<ProductListDto>> SearchProductsByNameAsync(string query)
    {
        var products = await productRepository.SearchByNameAsync(query);
        return products.Select(MapToProductListDto);
    }

    public async Task<IEnumerable<ProductListDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await productRepository.GetByCategoryAsync(categoryId);
        return products.Select(MapToProductListDto);
    }

    // НОВИЙ МЕТОД: Отримання схожих товарів
    public async Task<IEnumerable<ProductListDto>> GetRelatedProductsAsync(int categoryId, int currentProductId)
    {
        var products = await productRepository.GetByCategoryAsync(categoryId);

        return products
            .Where(p => p.ProductId != currentProductId)
            .OrderByDescending(p => p.ProductId)
            .Take(4)
            .Select(MapToProductListDto);
    }

    private static ProductListDto MapToProductListDto(Product p)
    {
        return new ProductListDto
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Price = p.Price,
            MainImageUrl = p.Images.Select(static i => i.ImageUrl).FirstOrDefault() ?? string.Empty,
            Rating = p.Rating
        };
    }

    private static ProductDetailsDto MapToProductDetailsDto(Product p)
    {
        return new ProductDetailsDto
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrls = p.Images.Select(static i => i.ImageUrl).ToList(),
            Rating = p.Rating,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty,
        };
    }
}