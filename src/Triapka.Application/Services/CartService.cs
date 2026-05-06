using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Application.Services;

public class CartService(ICartRepository cartRepository, IProductRepository productRepository) : ICartService
{
    public async Task<CartDto> GetCartAsync()
    {
        const int currentCustomerId = 1; // TODO: Замінити на реального юзера після Identity
        var cart = await cartRepository.GetCartByUserIdAsync(currentCustomerId);

        return cart == null ? new CartDto { CartItems = [], TotalAmount = 0 } : MapToCartDto(cart);
    }

    public async Task<CartDto> AddToCartAsync(int productId)
    {
        const int currentCustomerId = 1;
        _ = await productRepository.GetByIdAsync(productId)
                    ?? throw new InvalidOperationException("Product does not exist.");

        var cart = await cartRepository.GetCartByUserIdAsync(currentCustomerId)
                   ?? new Cart { UserId = currentCustomerId, CartItems = [] };

        var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                ProductId = productId,
                Quantity = 1
            });
        }

        var updatedCart = await cartRepository.UpdateCartAsync(cart);
        return MapToCartDto(updatedCart);
    }

    public async Task<CartDto> RemoveFromCartAsync(int cartItemId)
    {
        var updatedCart = await cartRepository.RemoveItemAsync(cartItemId);
        return updatedCart != null ? MapToCartDto(updatedCart) : new CartDto();
    }

    private static CartDto MapToCartDto(Cart cart)
    {
        return new CartDto
        {
            CartId = cart.CartId,
            CartItems = cart.CartItems.Select(i => new CartItemDto
            {
                ItemId = i.CartItemId,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Price = i.Product.Price,
                Quantity = i.Quantity,
                ImageUrl = i.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty
            }).ToList(),
            TotalAmount = cart.CartItems.Sum(i => i.Quantity * i.Product.Price)
        };
    }
}