namespace CatEats.Domain.ValueObjects;

public record MenuCategoryId(Guid Value)
{
    public static MenuCategoryId New() => new(Guid.NewGuid());
    public static implicit operator Guid(MenuCategoryId categoryId) => categoryId.Value;
}