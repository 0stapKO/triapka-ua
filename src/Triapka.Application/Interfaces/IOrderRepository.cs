using Triapka.Domain.Entities;

namespace Triapka.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);

    /// <summary>
    /// Returns all orders for a given user, sorted newest-first,
    /// with OrderItems and their Products eagerly loaded.
    /// </summary>
    Task<IReadOnlyList<Order>> GetByUserIdAsync(string userId);
}
