using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Application.Services;

public class CartService(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IHttpContextAccessor httpContextAccessor) : ICartService
{
    public async Task<CartDto> GetCartAsync()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return new CartDto { CartItems = [], TotalAmount = 0 };
        }

        var cart = await cartRepository.GetCartByUserIdAsync(userId);
        return cart == null ? new CartDto { CartItems = [], TotalAmount = 0 } : MapToCartDto(cart);
    }

    public async Task<CartDto> AddToCartAsync(int productId)
    {
        var userId = GetCurrentUserId()
            ?? throw new UnauthorizedAccessException("Користувач не авторизований.");

        _ = await productRepository.GetByIdAsync(productId)
                    ?? throw new InvalidOperationException("Товар не існує.");

        var cart = await cartRepository.GetCartByUserIdAsync(userId)
                   ?? new Cart { UserId = userId, CartItems = [] };

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
                ProductName = i.Product?.Name ?? "Unknown",
                Price = i.Product?.Price ?? 0,
                Quantity = i.Quantity,
                ImageUrl = i.Product?.Images?.FirstOrDefault()?.ImageUrl ?? string.Empty
            }).ToList(),
            TotalAmount = cart.CartItems.Sum(i => i.Quantity * (i.Product?.Price ?? 0))
        };
    }

    private string? GetCurrentUserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}