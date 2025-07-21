using CatEats.Domain.Common;
using CatEats.Domain.Exceptions;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.Entities;

public record MenuCategory : Entity<MenuCategoryId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    
    private readonly List<MenuItem> _menuItems = new();
    public IReadOnlyCollection<MenuItem> MenuItems => _menuItems.AsReadOnly();

    private MenuCategory() { } // EF Constructor

    public MenuCategory(string name, string description, int displayOrder) : base(MenuCategoryId.New())
    {
        Name = ValidateAndTrimString(name, nameof(name));
        Description = ValidateAndTrimString(description, nameof(description));
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public void AddMenuItem(string name, string description, decimal price, bool isAvailable)
    {
        if (_menuItems.Any(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new BusinessRuleValidationException($"Menu item '{name}' already exists in this category");

        var menuItem = new MenuItem(name, description, price, isAvailable);
        _menuItems.Add(menuItem);
    }

    public void UpdateDisplayOrder(int newOrder)
    {
        if (newOrder < 0)
            throw new ArgumentException("Display order cannot be negative", nameof(newOrder));
        DisplayOrder = newOrder;
    }

    private static string ValidateAndTrimString(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be empty", paramName);
        return value.Trim();
    }
}