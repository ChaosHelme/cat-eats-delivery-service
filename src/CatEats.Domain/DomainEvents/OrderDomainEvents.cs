using CatEats.Domain.Common;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.DomainEvents;

public record OrderCreatedEvent(OrderId OrderId, UserId CustomerId, RestaurantId RestaurantId) : IDomainEvent;
public record OrderItemAddedEvent(OrderId OrderId, MenuItemId MenuItemId, int Quantity) : IDomainEvent;
public record OrderItemRemovedEvent(OrderId OrderId, MenuItemId MenuItemId) : IDomainEvent;
public record OrderItemQuantityUpdatedEvent(OrderId OrderId, MenuItemId MenuItemId, int NewQuantity) : IDomainEvent;
public record OrderPlacedEvent(OrderId OrderId, UserId CustomerId, RestaurantId RestaurantId, decimal TotalAmount) : IDomainEvent;
public record OrderConfirmedEvent(OrderId OrderId, DateTime EstimatedDeliveryTime) : IDomainEvent;
public record OrderPreparationStartedEvent(OrderId OrderId) : IDomainEvent;
public record OrderReadyForPickupEvent(OrderId OrderId) : IDomainEvent;
public record OrderAssignedToRiderEvent(OrderId OrderId, UserId RiderId) : IDomainEvent;
public record OrderDeliveredEvent(OrderId OrderId, DateTime DeliveryTime) : IDomainEvent;
public record OrderCancelledEvent(OrderId OrderId, string Reason) : IDomainEvent;