using Domain;
using Domain.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DomainEventsDispatching;

public class DomainEventsAccessor
{
    public IReadOnlyCollection<IAmADomainEvent> GetAllDomainEvents(DbContext DbContext)
    {
        var domainEntities = DbContext.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Count != 0).ToList();

        return domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();
    }

    public void ClearAllDomainEvents(DbContext DbContext)
    {
        var domainEntities = DbContext.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Count != 0).ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());
    }
}