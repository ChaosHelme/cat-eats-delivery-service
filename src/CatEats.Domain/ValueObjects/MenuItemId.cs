namespace CatEats.Domain.ValueObjects;

public record MenuItemId(Guid Value)
{
    public static MenuItemId New() => new(Guid.NewGuid());
    public static implicit operator Guid(MenuItemId itemId) => itemId.Value;
}