using Domain.DomainEvents;

namespace Domain.Entities;

public abstract class Entity(Guid id)
{
    public Guid Id { get; } = id;
    
    private readonly List<IAmADomainEvent> _domainEvents = [];

    [System.Text.Json.Serialization.JsonIgnore]
    public IReadOnlyCollection<IAmADomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    
    public void AddDomainEvent(IAmADomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}