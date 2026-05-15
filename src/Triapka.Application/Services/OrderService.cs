using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Application.Services;

public class OrderService(
    ICartRepository cartRepository,
    IOrderRepository orderRepository,
    IHttpContextAccessor httpContextAccessor) : IOrderService
{
    public async Task<OrderResultDto> CreateOrderAsync(CheckoutDto dto)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Користувач не авторизований.");

        var cart = await cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null || cart.CartItems.Count == 0)
        {
            throw new InvalidOperationException("Кошик порожній.");
        }

        if (cart.CartItems.Any(i => i.Product == null))
        {
            throw new InvalidOperationException("Не вдалося отримати актуальні ціни товарів.");
        }

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalPrice = cart.CartItems.Sum(i => i.Quantity * i.Product.Price),
            Status = "Створено",
            ShippingAddress = dto.ShippingAddress.Trim(),
            Phone = dto.Phone.Trim(),
            OrderItems = cart.CartItems.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                PriceAtPurchase = i.Product.Price
            }).ToList()
        };

        var savedOrder = await orderRepository.CreateAsync(order);
        await cartRepository.ClearCartAsync(userId);

        return new OrderResultDto
        {
            OrderId = savedOrder.OrderId,
            OrderDate = savedOrder.OrderDate,
            TotalPrice = savedOrder.TotalPrice,
            Status = savedOrder.Status,
            TotalItems = savedOrder.OrderItems.Sum(i => i.Quantity)
        };
    }

    public async Task<IReadOnlyList<OrderDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await orderRepository.GetByUserIdAsync(userId);

        return orders.Select(o => new OrderDto
        {
            OrderId = o.OrderId,
            OrderDate = o.OrderDate,
            TotalPrice = o.TotalPrice,
            Status = o.Status,
            ShippingAddress = o.ShippingAddress,
            Phone = o.Phone,
            Items = o.OrderItems.Select(oi => new OrderItemDto
            {
                OrderItemId = oi.OrderItemId,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "Невідомий товар",
                ImageUrl = oi.Product?.Images?.FirstOrDefault()?.ImageUrl
                         ?? oi.Product?.ImageUrl,
                Quantity = oi.Quantity,
                PriceAtPurchase = oi.PriceAtPurchase
            }).ToList()
        }).ToList();
    }
}
