using Triapka.Application.DTOs;

namespace Triapka.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResultDto> CreateOrderAsync(CheckoutDto dto);

    /// <summary>
    /// Returns the order history for a given user, newest first.
    /// </summary>
    Task<IReadOnlyList<OrderDto>> GetUserOrdersAsync(string userId);
}
