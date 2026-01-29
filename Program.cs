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

// Serviços
builder.Services.AddSingleton<IOrderService, OrderService>();

// Controladores
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
