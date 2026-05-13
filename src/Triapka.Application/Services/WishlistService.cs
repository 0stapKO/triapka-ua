using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;

namespace Triapka.Application.Services;

public class WishlistService(
    IWishlistRepository wishlistRepository,
    IProductRepository productRepository,
    IHttpContextAccessor httpContextAccessor) : IWishlistService
{
    public async Task<IEnumerable<WishlistItemDto>> GetWishlistAsync()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return [];
        }

        var items = await wishlistRepository.GetByUserIdAsync(userId);

        return items.Select(MapToWishlistItemDto);
    }

    public async Task<WishlistItemDto> AddToWishlistAsync(int productId)
    {
        var userId = GetCurrentUserId()
            ?? throw new UnauthorizedAccessException("Користувач не авторизований.");

        _ = await productRepository.GetByIdAsync(productId)
                      ?? throw new InvalidOperationException("Product does not exist.");

        var item = await wishlistRepository.AddAsync(userId, productId);
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

    private string? GetCurrentUserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}