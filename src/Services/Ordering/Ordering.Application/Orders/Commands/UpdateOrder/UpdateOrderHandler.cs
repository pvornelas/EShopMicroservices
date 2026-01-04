namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    public class UpdateOrderHandler(IApplicationDbContext dbContext)
        : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
    {
        public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            var orderId = OrderId.Of(command.Order.Id);
            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order is null)
                throw new OrderNotFoundException(command.Order.Id);

            UpdateOrderWithNewValues(order, command.Order);

            //dbContext.Orders.Update(order);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateOrderResult(true);

        }

        private void UpdateOrderWithNewValues(Order order, OrderDto orderDto)
        {
            var shippingAddress = Address.Of(
                fisrtName: orderDto.ShippingAddress.FirstName,
                lastName: orderDto.ShippingAddress.LastName,
                emailAdress: orderDto.ShippingAddress.EmailAddress,
                addressLine: orderDto.ShippingAddress.AddressLine,
                country: orderDto.ShippingAddress.Country,
                state: orderDto.ShippingAddress.State,
                zipCode: orderDto.ShippingAddress.ZipCode
            );

            var billingAddress = Address.Of(
                fisrtName: orderDto.BillingAddress.FirstName,
                lastName: orderDto.BillingAddress.LastName,
                emailAdress: orderDto.BillingAddress.EmailAddress,
                addressLine: orderDto.BillingAddress.AddressLine,
                country: orderDto.BillingAddress.Country,
                state: orderDto.BillingAddress.State,
                zipCode: orderDto.BillingAddress.ZipCode
            );

            var payment = Payment.Of(
                cardName: orderDto.Payment.CardName,
                cardNumber: orderDto.Payment.CardNumber,
                expiration: orderDto.Payment.Expiration,
                cvv: orderDto.Payment.Cvv,
                paymentMethod: orderDto.Payment.PaymentMethod
            );

            order.Update(
                orderName: OrderName.Of(orderDto.OrderName),
                shippingAdress: shippingAddress,
                billingAdress: billingAddress,
                payment: payment,
                status: orderDto.Status
            );

            SyncOrderItems(order, orderDto.OrderItems);
        }

        private static void SyncOrderItems(Order order, IReadOnlyCollection<OrderItemDto> incomingItems)
        {
            // Falha cedo se vier ProductId duplicado no payload
            var incomingByProduct = incomingItems.ToDictionary(i => i.ProductId);

            // Indexa os existentes por ProductId (Guid)
            var existingByProduct = order.OrderItems.ToDictionary(oi => oi.ProductId.Value);

            // Sets de comparação
            var incomingKeys = incomingByProduct.Keys;
            var existingKeys = existingByProduct.Keys;

            // 1) Remover: existia antes e não veio mais
            foreach (var productIdGuid in existingKeys.Except(incomingKeys))
            {
                var existing = existingByProduct[productIdGuid];
                order.Remove(existing.ProductId);
            }

            // 2) Atualizar: existe dos dois lados, mas qty/preço mudou
            foreach (var productIdGuid in existingKeys.Intersect(incomingKeys))
            {
                var existing = existingByProduct[productIdGuid];
                var dto = incomingByProduct[productIdGuid];

                if (existing.Quantity != dto.Quantity || existing.Price != dto.Price)
                {
                    // Seu domínio não tem UpdateItem, então é remove + add
                    order.Remove(existing.ProductId);
                    order.Add(existing.ProductId, dto.Quantity, dto.Price);
                }
            }

            // 3) Adicionar: veio no payload mas não existia
            foreach (var productIdGuid in incomingKeys.Except(existingKeys))
            {
                var dto = incomingByProduct[productIdGuid];
                order.Add(ProductId.Of(productIdGuid), dto.Quantity, dto.Price);
            }
        }
    }
}
