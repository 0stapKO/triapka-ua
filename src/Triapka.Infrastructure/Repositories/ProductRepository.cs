using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Infrastructure.Repositories
{
    public class ProductRepository(ApplicationDbContext context) : IProductRepository
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
            if (string.IsNullOrWhiteSpace(query))
            {
                return await context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Images)
                    .ToListAsync();
            }

            var clearedQuery = query.Trim();
            var processedQuery = Regex.Replace(clearedQuery, @"\s+", "%");

            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => EF.Functions.ILike(p.Name, $"%{processedQuery}%"))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Images)
                .ToListAsync();
        }
    }
}