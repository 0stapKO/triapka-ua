using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;

public interface IWishlistRepository
{
    public Task<IEnumerable<WishlistItem>> GetByUserIdAsync(int userId);

    public Task<WishlistItem> AddAsync(int userId, int productId);

    public Task<WishlistItem> RemoveAsync(int wishlistItemId);
}