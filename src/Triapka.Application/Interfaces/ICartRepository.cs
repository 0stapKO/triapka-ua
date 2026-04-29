using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;
public interface ICartRepository
{
    public Task<Cart> GetCartByUserIdAsync(int userId);

    public Task<Cart> AddItemAsync(CartItem item);

    public Task<Cart> RemoveItemAsync(int itemId);
}
