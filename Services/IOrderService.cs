using OrdersApi.Models;

namespace OrdersApi.Services;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(string id);
    Task<Order> CreateAsync(Order order);
    Task<bool> UpdateAsync(string id, Order order);
    Task<bool> DeleteAsync(string id);
}