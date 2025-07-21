using Mediator;

namespace CatEats.Domain;

public abstract record Entity
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool Deleted { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();
    readonly List<INotification> _domainEvents = [];

    protected void AddDomainEvent(INotification eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}