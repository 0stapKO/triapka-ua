namespace Triapka.Application.Services;
using Triapka.Application.Interfaces;
using Triapka.Application.DTOs;
using Triapka.Domain.Entities;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<IEnumerable<ProductListDto>> GetAllProductsAsync()
    {
        var products = await productRepository.GetAllAsync();
        return products.Select(MapToProductListDto);
    }

    public async Task<ProductDetailsDto?> GetProductByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        return product == null ? null : MapToProductDetailsDto(product);
    }

    public async Task<IEnumerable<ProductListDto>> SearchProductsByNameAsync(string query)
    {
        var products = await productRepository.SearchByNameAsync(query);
        return products.Select(MapToProductListDto);
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
            Price = p.Price,
            ImageUrls = p.Images.Select(static i => i.ImageUrl).ToList(),
            Rating = p.Rating,
            CategoryName = p.Category?.Name ?? string.Empty,
        };
    }
}
