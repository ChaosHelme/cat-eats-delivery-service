namespace CatEats.Domain.Enumerations;

public enum DeliveryStatus
{
    Assigned = 1,
    EnRouteToPickup = 2,
    PickedUp = 3,
    EnRouteToCustomer = 4,
    Delivered = 5,
    Cancelled = 6
}