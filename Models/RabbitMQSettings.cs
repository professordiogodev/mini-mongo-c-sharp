namespace OrdersApi.Models;

public class RabbitMQSettings
{
    public string HostName { get; set; } = string.Empty;
    public int Port { get; set; } = 5672;  // This is correct as int
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}