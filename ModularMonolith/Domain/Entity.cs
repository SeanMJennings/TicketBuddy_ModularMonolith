using Domain.DomainEvents;

namespace Domain;

public abstract class Entity(Guid Id)
{
    public Guid Id { get; } = Id;
    
    private List<IAmADomainEvent> _domainEvents = [];

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