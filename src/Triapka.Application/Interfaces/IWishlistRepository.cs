using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;

public interface IWishlistRepository
{
    Task<IEnumerable<WishlistItem>> GetByUserIdAsync(int userId);

    Task<WishlistItem> AddAsync(int userId, int productId);

    Task<WishlistItem> RemoveAsync(int wishlistItemId);
}