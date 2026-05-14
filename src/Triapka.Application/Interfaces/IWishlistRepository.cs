using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;

public interface IWishlistRepository
{
    Task<IEnumerable<WishlistItem>> GetByUserIdAsync(string userId);

    Task<(WishlistItem Item, bool IsNew)> AddAsync(string userId, int productId);

    Task<WishlistItem?> RemoveAsync(int wishlistItemId);
}