using Microsoft.EntityFrameworkCore;

using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    public async Task<IEnumerable<ProductCategory>> GetAllAsync()
    {
        return await context.ProductCategories.ToListAsync();
    }
}