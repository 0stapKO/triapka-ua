using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<ProductCategory>> GetAllAsync();
}