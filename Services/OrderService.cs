using System.Runtime.CompilerServices;
using MongoDB.Driver;
using OrdersApi.Models;

namespace OrdersApi.Services;

public class OrderService : IOrderService
{
    // Dependency Inversion Principle
    // Injetar as nossas settings para este serviço poder existir
    private readonly IMongoCollection<Order> _orders;

    public OrderService(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _orders = database.GetCollection<Order>("orders");
    }

    // Read All
    public async Task<List<Order>> GetAllAsync()
    {
        return await _orders.Find(_ => true).ToListAsync();
    }

    // Read One
    public async Task<Order?> GetByIdAsync(string id)
    {
        return await _orders.Find(order => order.Id == id).FirstOrDefaultAsync();
    }

    // Create
    public async Task<Order> CreateAsync(Order order)
    {
        // Gerar um número de Order
        order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        await _orders.InsertOneAsync(order);
        return order;
    }

    // Update
    public async Task<bool> UpdateAsync(string id, Order order)
    {
        order.UpdatedAt = DateTime.UtcNow;

        // Inserir a order a ser alterada [o] e a nova [order]
        var result = await _orders.ReplaceOneAsync(
            o => o.Id == id,
            order
        );

        return result.ModifiedCount > 0;
    }

    // Delete
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _orders.DeleteOneAsync(order => order.Id == id);
        return result.DeletedCount > 0;
    }
}