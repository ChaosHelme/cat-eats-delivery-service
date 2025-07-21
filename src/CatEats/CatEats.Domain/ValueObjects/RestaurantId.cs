namespace CatEats.Domain.ValueObjects;

public record RestaurantId(Guid Value)
{
    public static RestaurantId New() => new(Guid.NewGuid());
    public static implicit operator Guid(RestaurantId restaurantId) => restaurantId.Value;
}