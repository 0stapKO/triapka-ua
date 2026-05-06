using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetCartByUserIdAsync(string userId);

    Task<Cart> AddItemAsync(CartItem item);

    Task<Cart?> RemoveItemAsync(int itemId);

    Task<Cart> UpdateCartAsync(Cart cart);
}
