using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
}
