using CatEats.Domain.Common;
using CatEats.Domain.Enumerations;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.DomainEvents;

public record UserRegisteredEvent(UserId UserId, string Email, UserRole Role) : IDomainEvent;
public record AddressAddedEvent(UserId UserId, Address Address) : IDomainEvent;
public record UserDeactivatedEvent(UserId UserId) : IDomainEvent;