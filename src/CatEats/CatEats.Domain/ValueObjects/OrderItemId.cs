namespace CatEats.Domain.ValueObjects;

public record OrderItemId(Guid Value)
{
    public static OrderItemId New() => new(Guid.NewGuid());
    public static implicit operator Guid(OrderItemId orderItemId) => orderItemId.Value;
}