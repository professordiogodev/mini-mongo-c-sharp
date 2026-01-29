using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using OrdersApi.Models;
using OrdersApi.Services;

var builder = WebApplication.CreateBuilder(args);

// MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB")
);

builder.Services.AddSingleton<MongoDbSettings>(
    sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value
);

// RabbitMQ
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ")
);

builder.Services.AddSingleton<RabbitMQSettings>(
    sp => sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value
);

// Serviços
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

// Controladores
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
