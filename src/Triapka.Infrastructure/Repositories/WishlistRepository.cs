using Microsoft.EntityFrameworkCore;

using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Infrastructure.Repositories;

public class WishlistRepository(ApplicationDbContext context) : IWishlistRepository
{
    public async Task<IEnumerable<WishlistItem>> GetByUserIdAsync(string userId)
    {
        return await context.WishlistItems
            .Where(w => w.UserId == userId)
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .ToListAsync();
    }

    public async Task<WishlistItem> AddAsync(string userId, int productId)
    {
        var existingItem = await context.WishlistItems
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (existingItem != null)
        {
            return existingItem;
        }

        var entry = await context.WishlistItems.AddAsync(new WishlistItem
        {
            UserId = userId,
            ProductId = productId
        });

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return await context.WishlistItems
                .Include(w => w.Product)
                    .ThenInclude(p => p.Images)
                .FirstAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        return await context.WishlistItems
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .FirstAsync(w => w.WishlistItemId == entry.Entity.WishlistItemId);
    }

    public async Task<WishlistItem?> RemoveAsync(int wishlistItemId)
    {
        var entity = await context.WishlistItems
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(w => w.WishlistItemId == wishlistItemId);

        if (entity == null)
        {
            return null;
        }

        context.WishlistItems.Remove(entity);
        await context.SaveChangesAsync();

        return entity;
    }
}