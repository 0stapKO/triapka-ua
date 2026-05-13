using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;

public interface IWishlistRepository
{
    Task<IEnumerable<WishlistItem>> GetByUserIdAsync(string userId);

    Task<WishlistItem> AddAsync(string userId, int productId);

    Task<WishlistItem?> RemoveAsync(int wishlistItemId);
}