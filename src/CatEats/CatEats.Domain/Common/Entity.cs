namespace CatEats.Domain.Common;

public abstract record Entity<TId>
{
    public TId Id { get; protected set; } = default!;

    protected Entity(TId id)
    {
        Id = id;
    }

    protected Entity() { }
}