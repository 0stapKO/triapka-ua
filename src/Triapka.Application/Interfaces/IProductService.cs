using Triapka.Application.DTOs;

namespace Triapka.Application.Interfaces;
public interface IProductService
{
    Task<IEnumerable<ProductListDto>> GetAllProductsAsync();

    Task<ProductDetailsDto?> GetProductByIdAsync(int id);

    Task<IEnumerable<ProductListDto>> SearchProductsByNameAsync(string query);
}
