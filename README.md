# Pequeno Exercício

Vou pedir que clonem o projeto e o corram com o Visual Studio.

Depois, devem utilizar o `Postman` ou `curl` para criar um objeto na API:

```POST http://localhost:5124/api/orders```

Body:
```
{
  "customerName": "John Doe",
  "customerEmail": "john@example.com",
  "totalAmount": 199.99,
  "status": "Pending",
  "items": [
    {
      "productName": "Laptop",
      "quantity": 1,
      "unitPrice": 199.99
    }
  ]
}
```

E depois utilizarem o browser ou o próprio postman para visualziar o pedido inserido.