using Triapka.Application.DTOs;
using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
}