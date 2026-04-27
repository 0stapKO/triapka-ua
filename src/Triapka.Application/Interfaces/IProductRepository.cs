using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<IEnumerable<Product>> SearchByNameAsync(string query);
}
