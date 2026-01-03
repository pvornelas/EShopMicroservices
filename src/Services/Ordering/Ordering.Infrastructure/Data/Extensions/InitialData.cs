namespace Ordering.Infrastructure.Data.Extensions
{
    internal class InitialData
    {
        public static IEnumerable<Customer> Customers =>
          new List<Customer>
            {
                Customer.Create(
                    CustomerId.Of(Guid.Parse("8f5f3e9a-6a4f-4d6d-9c1f-1d8f3a2b4c10")),
                    "Ana Souza",
                    "ana.souza@email.com"
                ),
                Customer.Create(
                    CustomerId.Of(Guid.Parse("1b2c3d4e-5f60-4a7b-8c9d-0e1f2a3b4c5d")),
                    "Bruno Lima",
                    "bruno.lima@email.com"
                ),
                Customer.Create(
                    CustomerId.Of(Guid.Parse("a2b3c4d5-e6f7-4a8b-9c0d-1e2f3a4b5c6d")),
                    "Carla Martins",
                    "carla.martins@email.com"
                )
            };

        public static IEnumerable<Product> Products =>
            new List<Product>
            {
                Product.Create(
                    ProductId.Of(Guid.Parse("c1a0d7b2-2f7a-4a3b-8b2c-1a1f7d6c0e11")),
                    name: "Teclado Mecânico",
                    price: 399.90m
                ),

                Product.Create(
                    ProductId.Of(Guid.Parse("d2b1e8c3-3a8b-4b4c-9c3d-2b2e8c7d1f22")),
                    name: "Mouse Gamer",
                    price: 189.90m
                ),

                Product.Create(
                    ProductId.Of(Guid.Parse("e3c2f9d4-4b9c-4c5d-ad4e-3c3f9d8e2a33")),
                    name: "Headset",
                    price: 279.90m
                )
            };

        public static IEnumerable<Order> OrdersWithItems =>
            new List<Order>
            {
                new Func<Order>(() =>
                {
                    var order = Order.Create(
                        id: OrderId.Of(Guid.Parse("11111111-1111-1111-1111-111111111111")),
                        customerId: CustomerId.Of(Guid.Parse("8f5f3e9a-6a4f-4d6d-9c1f-1d8f3a2b4c10")),
                        orderName: OrderName.Of("OR001"),
                        shippingAddress: Address.Of(
                            fisrtName: "Ana",
                            lastName: "Souza",
                            emailAdress: "ana.souza@email.com",
                            addressLine: "Rua das Flores, 123",
                            country: "BR",
                            state: "SP",
                            zipCode: "04567-000"
                        ),
                        billingAddress: Address.Of(
                            fisrtName: "Ana",
                            lastName: "Souza",
                            emailAdress: "ana.souza@email.com",
                            addressLine: "Rua das Flores, 123",
                            country: "BR",
                            state: "SP",
                            zipCode: "04567-000"
                        ),
                        payment: Payment.Of(
                            cardName: "ANA SOUZA",
                            cardNumber: "4111111111111111",
                            expiration: "12/29",
                            cvv: "123",
                            paymentMethod: 1
                        )
                    );

                    // mesmos Products seedados
                    order.Add(ProductId.Of(Guid.Parse("c1a0d7b2-2f7a-4a3b-8b2c-1a1f7d6c0e11")), quantity: 1, price: 399.90m);
                    order.Add(ProductId.Of(Guid.Parse("d2b1e8c3-3a8b-4b4c-9c3d-2b2e8c7d1f22")), quantity: 2, price: 189.90m);

                    return order;
                })(),

                new Func<Order>(() =>
                {
                    var order = Order.Create(
                        id: OrderId.Of(Guid.Parse("22222222-2222-2222-2222-222222222222")),
                        customerId: CustomerId.Of(Guid.Parse("1b2c3d4e-5f60-4a7b-8c9d-0e1f2a3b4c5d")), // mesmo Customer 2
                        orderName: OrderName.Of("OR002"),
                        shippingAddress: Address.Of(
                            fisrtName: "Bruno",
                            lastName: "Lima",
                            emailAdress: "bruno.lima@email.com",
                            addressLine: "Av. Paulista, 1000",
                            country: "BR",
                            state: "SP",
                            zipCode: "01310-100"
                        ),
                        billingAddress: Address.Of(
                            fisrtName: "Bruno",
                            lastName: "Lima",
                            emailAdress: "bruno.lima@email.com",
                            addressLine: "Av. Paulista, 1000",
                            country: "BR",
                            state: "SP",
                            zipCode: "01310-100"
                        ),
                        payment: Payment.Of(
                            cardName: "BRUNO LIMA",
                            cardNumber: "5555555555554444",
                            expiration: "08/30",
                            cvv: "987",
                            paymentMethod: 2
                        )
                    );

                    // mesmos Products seedados
                    order.Add(ProductId.Of(Guid.Parse("d2b1e8c3-3a8b-4b4c-9c3d-2b2e8c7d1f22")), quantity: 1, price: 189.90m);
                    order.Add(ProductId.Of(Guid.Parse("e3c2f9d4-4b9c-4c5d-ad4e-3c3f9d8e2a33")), quantity: 1, price: 279.90m);

                    return order;
                })()
            };

    }
}
