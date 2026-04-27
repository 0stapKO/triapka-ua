using Microsoft.EntityFrameworkCore;
using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Infrastructure.Repositories
{
    internal class ProductRepository(ApplicationDbContext context) : IProductRepository
    {
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string query)
        {
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.Name.Contains(query))
                .ToListAsync();
        }
    }
}
