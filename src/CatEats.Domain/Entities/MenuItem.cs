using CatEats.Domain.Common;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.Entities;

public record MenuItem : Entity<MenuItemId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private MenuItem() { } // EF Constructor

    public MenuItem(string name, string description, decimal price, bool isAvailable) : base(MenuItemId.New())
    {
        Name = ValidateAndTrimString(name, nameof(name));
        Description = ValidateAndTrimString(description, nameof(description));
        Price = ValidatePrice(price);
        IsAvailable = isAvailable;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal newPrice)
    {
        Price = ValidatePrice(newPrice);
    }

    public void SetAvailability(bool available)
    {
        IsAvailable = available;
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
}