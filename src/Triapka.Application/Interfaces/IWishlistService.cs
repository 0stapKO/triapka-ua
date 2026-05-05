using Triapka.Application.DTOs;

namespace Triapka.Application.Interfaces;
public interface IWishlistService
{
    Task<IEnumerable<WishlistItemDto>> GetWishlistAsync();

    Task<WishlistItemDto> AddToWishlistAsync(int productId);

    Task RemoveFromWishlistAsync(int wishlistItemId);
}