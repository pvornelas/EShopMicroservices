namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    public class UpdateOrderHandler(IApplicationDbContext dbContext)
        : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
    {
        public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            var orderId = OrderId.Of(command.Order.Id);
            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order is null)
                throw new OrderNotFoundException(command.Order.Id);

            UpdateOrderWithNewValues(order, command.Order);

            dbContext.Orders.Update(order);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateOrderResult(true);

        }

        private void UpdateOrderWithNewValues(Order order, OrderDto orderDto)
        {
            var shippingAddress = Address.Of(
                fisrtName: orderDto.ShippingAddress.FirstName,
                lastName: orderDto.ShippingAddress.LastName,
                emailAdress: orderDto.ShippingAddress.EmailAdress,
                addressLine: orderDto.ShippingAddress.AddressLine,
                country: orderDto.ShippingAddress.Country,
                state: orderDto.ShippingAddress.State,
                zipCode: orderDto.ShippingAddress.ZipCode
            );

            var billingAddress = Address.Of(
                fisrtName: orderDto.BillingAddress.FirstName,
                lastName: orderDto.BillingAddress.LastName,
                emailAdress: orderDto.BillingAddress.EmailAdress,
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
        }
    }
}
