using BuildingBlocks.Messaging.Events;
using MassTransit;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.Application.Orders.EventHandlers.Integration
{
    public class BasketCheckoutEventHandler(ISender sender, ILogger<BasketCheckoutEventHandler> logger)
        : IConsumer<BasketCheckoutEvent>
    {
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            // TODO: Create new order and start order fullfillment process
            logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

            var command = MapToCreateOrderCommand(context.Message);
            await sender.Send(command);
        }

        private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent message)
        {
            // Create full order with incoming event data
            var addressDto = new AddressDto(
                FirstName: message.FirstName,
                LastName: message.LastName,
                EmailAddress: message.EmailAddress,
                AddressLine: message.AddressLine,
                Country: message.Country,
                State: message.State,
                ZipCode: message.ZipCode
            );

            var paymentDto = new PaymentDto(
                CardName: message.CardName,
                CardNumber: message.CardNumber,
                Expiration: message.Expiration,
                Cvv: message.CVV,
                PaymentMethod: message.PaymentMethod
            );

            var orderId = Guid.NewGuid();

            var orderDto = new OrderDto(
                Id: orderId,
                CustomerId: message.CustomerId,
                OrderName: message.UserName,
                ShippingAddress: addressDto,
                BillingAddress: addressDto,
                Payment: paymentDto,
                Status: OrderStatus.Pending,
                OrderItems: [
                    new OrderItemDto(orderId, new Guid("e3c2f9d4-4b9c-4c5d-ad4e-3c3f9d8e2a33"), 1, 279.90m),
                    new OrderItemDto(orderId, new Guid("d2b1e8c3-3a8b-4b4c-9c3d-2b2e8c7d1f22"), 1, 189.90m)
                ]
            );

            return new CreateOrderCommand(orderDto);
        }
    }
}
