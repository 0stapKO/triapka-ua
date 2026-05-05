using Triapka.Application.DTOs;

namespace Triapka.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync();

    Task<CartDto> AddToCartAsync(int productId);

    Task<CartDto> RemoveFromCartAsync(int cartItemId);
}