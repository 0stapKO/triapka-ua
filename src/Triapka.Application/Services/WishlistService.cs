using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;

namespace Triapka.Application.Services;

public class WishlistService(IWishlistRepository wishlistRepository, IProductRepository productRepository) : IWishlistService
{
    public async Task<IEnumerable<WishlistItemDto>> GetWishlistAsync()
    {
        const int currentCustomerId = 1; // TODO: Identity
        var items = await wishlistRepository.GetByUserIdAsync(currentCustomerId);

        return items.Select(MapToWishlistItemDto);
    }

    public async Task<WishlistItemDto> AddToWishlistAsync(int productId)
    {
        const int currentCustomerId = 1;
        _ = await productRepository.GetByIdAsync(productId)
                      ?? throw new InvalidOperationException("Product does not exist.");

        var item = await wishlistRepository.AddAsync(currentCustomerId, productId);
        return MapToWishlistItemDto(item);
    }

    public async Task RemoveFromWishlistAsync(int wishlistItemId)
    {
        await wishlistRepository.RemoveAsync(wishlistItemId);
    }

    private static WishlistItemDto MapToWishlistItemDto(Domain.Entities.WishlistItem item)
    {
        return new WishlistItemDto
        {
            WishlistItemId = item.WishlistItemId,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Price = item.Product.Price,
            ImageUrl = item.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty
        };
    }
}