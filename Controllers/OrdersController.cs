using Microsoft.AspNetCore.Mvc;
using OrdersApi.Models;
using OrdersApi.Services;
using System.Text.Json;

namespace OrdersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderService orderService,
        IMessagePublisher messagePublisher,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetAll()
    {
        try
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500, "An error occurred while retrieving orders");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetById(string id)
    {
        try
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
                return NotFound($"Order with ID {id} not found");

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderId}", id);
            return StatusCode(500, "An error occurred while retrieving the order");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create([FromBody] Order order)
    {
        try
        {
            // Save to database
            var createdOrder = await _orderService.CreateAsync(order);

            // Publish message to RabbitMQ
            var message = JsonSerializer.Serialize(new
            {
                EventType = "OrderCreated",
                OrderId = createdOrder.Id,
                OrderNumber = createdOrder.OrderNumber,
                CustomerName = createdOrder.CustomerName,
                TotalAmount = createdOrder.TotalAmount,
                Timestamp = DateTime.UtcNow
            });

            _messagePublisher.PublishMessage(
                exchangeName: "orders.events",
                routingKey: "order.created",
                message: message
            );

            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, "An error occurred while creating the order");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, [FromBody] Order order)
    {
        try
        {
            var updated = await _orderService.UpdateAsync(id, order);

            if (!updated)
                return NotFound($"Order with ID {id} not found");

            // Publish message to RabbitMQ
            var message = JsonSerializer.Serialize(new
            {
                EventType = "OrderUpdated",
                OrderId = id,
                Timestamp = DateTime.UtcNow
            });

            _messagePublisher.PublishMessage(
                exchangeName: "orders.events",
                routingKey: "order.updated",
                message: message
            );

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId}", id);
            return StatusCode(500, "An error occurred while updating the order");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            var deleted = await _orderService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Order with ID {id} not found");

            // Publish message to RabbitMQ
            var message = JsonSerializer.Serialize(new
            {
                EventType = "OrderDeleted",
                OrderId = id,
                Timestamp = DateTime.UtcNow
            });

            _messagePublisher.PublishMessage(
                exchangeName: "orders.events",
                routingKey: "order.deleted",
                message: message
            );

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order {OrderId}", id);
            return StatusCode(500, "An error occurred while deleting the order");
        }
    }
}
