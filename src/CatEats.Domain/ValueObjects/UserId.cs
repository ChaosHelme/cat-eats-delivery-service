namespace CatEats.Domain.ValueObjects;

public record UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static implicit operator Guid(UserId userId) => userId.Value;
}