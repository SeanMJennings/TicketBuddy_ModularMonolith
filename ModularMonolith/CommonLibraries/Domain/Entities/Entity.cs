using Domain.DomainEvents;

namespace Domain.Entities;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        Validation.BasedOn(errors =>
        {
            if (id == Guid.Empty)
            {
                errors.Add("Entity ID cannot be an empty GUID.");
            }
        });
        Id = id;
    }
    
    public Guid Id { get; }
    
    private readonly List<IDescribeADomainEvent> _domainEvents = [];

    [System.Text.Json.Serialization.JsonIgnore]
    public IReadOnlyCollection<IDescribeADomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void AddDomainEvent(IDescribeADomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}