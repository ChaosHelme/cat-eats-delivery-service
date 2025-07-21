namespace CatEats.Domain.ValueObjects;

public record OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.NewGuid());
    public static implicit operator Guid(OrderId orderId) => orderId.Value;
}