namespace CatEats.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn => DateTime.UtcNow;
}
