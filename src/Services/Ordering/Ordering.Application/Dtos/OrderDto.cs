using Ordering.Domain.Enum;

namespace Ordering.Application.Dtos
{
    public record OrderDto(
        Guid Id,
        Guid CustomerId,
        string OrderName,
        AddressDto ShippingAddress,
        AddressDto BillingAdress,
        PaymentDto Payment,
        OrderStatus Status,
        List<OrderItemDto> OrderItems);
}
