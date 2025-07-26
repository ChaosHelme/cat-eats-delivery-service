using CatEats.Domain.Common;
using CatEats.Domain.ValueObjects;
using CatEats.UserService.Domain.Enumerations;

namespace CatEats.UserService.Domain.DomainEvents;

public record UserRegisteredEvent(UserId UserId, string Email, UserRole Role) : IDomainEvent;
public record AddressAddedEvent(UserId UserId, Address Address) : IDomainEvent;
public record UserDeactivatedEvent(UserId UserId) : IDomainEvent;