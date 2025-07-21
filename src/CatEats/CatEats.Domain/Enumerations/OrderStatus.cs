namespace CatEats.Domain.Enumerations;

public enum OrderStatus
{
    Created = 1,
    Placed = 2,
    Confirmed = 3,
    InPreparation = 4,
    ReadyForPickup = 5,
    OutForDelivery = 6,
    Delivered = 7,
    Cancelled = 8
}