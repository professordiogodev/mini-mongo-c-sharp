using OrdersApi.Models;

namespace OrdersApi.Services;

public interface IMessagePublisher
{
    void PublishMessage(string exchangeName, string routingKey, string message);
}