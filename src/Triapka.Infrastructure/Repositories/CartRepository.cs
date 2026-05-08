using Microsoft.EntityFrameworkCore;

using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Infrastructure.Repositories;

public class CartRepository(ApplicationDbContext context) : ICartRepository
{
    public async Task<Cart?> GetCartByUserIdAsync(string userId)
    {
        return await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart> AddItemAsync(CartItem item)
    {
        await context.AddAsync(item);
        await context.SaveChangesAsync();

        return await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Images)
            .FirstAsync(c => c.CartId == item.CartId);
    }

    public async Task<Cart?> RemoveItemAsync(int itemId)
    {
        var itemToRemove = await context.Set<CartItem>().FindAsync(itemId);

        if (itemToRemove == null)
        {
            return null;
        }

        var targetCartId = itemToRemove.CartId;

        context.Remove(itemToRemove);
        await context.SaveChangesAsync();

        return await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.CartId == targetCartId);
    }

    public async Task<Cart> UpdateCartAsync(Cart cart)
    {
        context.Carts.Update(cart);
        await context.SaveChangesAsync();

        return await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Images)
            .FirstAsync(c => c.CartId == cart.CartId);
    }
}