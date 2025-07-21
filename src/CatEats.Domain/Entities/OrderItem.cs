using CatEats.Domain.Common;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.Entities;

public record OrderItem : Entity<OrderItemId>
{
    public MenuItemId MenuItemId { get; private set; }
    public string ItemName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;
    public string? SpecialRequests { get; private set; }

    private OrderItem() { } // EF Constructor

    public OrderItem(MenuItemId menuItemId, string itemName, decimal unitPrice, int quantity,
        string? specialRequests = null) : base(OrderItemId.New())
    {
        MenuItemId = menuItemId;
        ItemName = ValidateAndTrimString(itemName, nameof(itemName));
        UnitPrice = ValidatePrice(unitPrice);
        Quantity = ValidateQuantity(quantity);
        SpecialRequests = specialRequests?.Trim();
    }

    public void UpdateQuantity(int newQuantity)
    {
        Quantity = ValidateQuantity(newQuantity);
    }

    private static string ValidateAndTrimString(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be empty", paramName);
        return value.Trim();
    }

    private static decimal ValidatePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));
        return price;
    }

    private static int ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        return quantity;
    }
}