using CatEats.Domain.Common;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.DomainEvents;

public record RestaurantRegisteredEvent(RestaurantId RestaurantId, string Name, UserId OwnerId) : IDomainEvent;
public record RestaurantApprovedEvent(RestaurantId RestaurantId) : IDomainEvent;
public record RestaurantSuspendedEvent(RestaurantId RestaurantId) : IDomainEvent;
public record MenuCategoryAddedEvent(RestaurantId RestaurantId, MenuCategoryId CategoryId, string Name) : IDomainEvent;
public record MenuItemAddedEvent(RestaurantId RestaurantId, MenuCategoryId CategoryId, string ItemName, decimal Price) : IDomainEvent;