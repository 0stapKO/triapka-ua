using Triapka.Application.DTOs;

namespace Triapka.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResultDto> CreateOrderAsync(CheckoutDto dto);
}
