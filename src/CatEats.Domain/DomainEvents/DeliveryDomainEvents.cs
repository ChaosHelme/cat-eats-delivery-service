using CatEats.Domain.Common;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.DomainEvents;

public record DeliveryAssignedEvent(DeliveryId DeliveryId, OrderId OrderId, UserId RiderId) : IDomainEvent;
public record DeliveryStartedEvent(DeliveryId DeliveryId, UserId RiderId) : IDomainEvent;
public record OrderPickedUpEvent(DeliveryId DeliveryId, OrderId OrderId, DateTime PickupTime) : IDomainEvent;
public record DeliveryEnRouteToCustomerEvent(DeliveryId DeliveryId) : IDomainEvent;
public record DeliveryCompletedEvent(DeliveryId DeliveryId, OrderId OrderId, DateTime DeliveryTime) : IDomainEvent;
public record DeliveryCancelledEvent(DeliveryId DeliveryId, OrderId OrderId, string Reason) : IDomainEvent;
public record RiderLocationUpdatedEvent(DeliveryId DeliveryId, UserId RiderId, double Latitude, double Longitude, DateTime UpdateTime) : IDomainEvent;