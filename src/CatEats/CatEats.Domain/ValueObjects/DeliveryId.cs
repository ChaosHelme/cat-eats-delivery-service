namespace CatEats.Domain.ValueObjects;

public record DeliveryId(Guid Value)
{
    public static DeliveryId New() => new(Guid.NewGuid());
    public static implicit operator Guid(DeliveryId deliveryId) => deliveryId.Value;
}