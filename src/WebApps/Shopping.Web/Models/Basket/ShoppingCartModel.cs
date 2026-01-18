namespace Shopping.Web.Models.Basket;

public class ShoppingCartModel
{
    public string UserName { get; set; } = default!;
    public List<ShoppingCartItemModel> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
}

public class ShoppingCartItemModel
{
    public int Quantity { get; set; }
    public string Color { get; set; } = default!;
    public decimal Price { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
}

public record GetBasketResponse(ShoppingCartModel Cart);
public record StoreBasketRequest(ShoppingCartModel Cart);
public record StoreBasketResponse(string UserName);
public record DeleteBasketResponse(bool IsSuccess);