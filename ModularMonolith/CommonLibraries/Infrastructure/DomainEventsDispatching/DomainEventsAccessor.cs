using Domain;
using Domain.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DomainEventsDispatching;

public static class DomainEventsAccessor
{
    public static IReadOnlyCollection<IAmADomainEvent> GetAllDomainEvents(DbContext DbContext)
    {
        var domainEntities = DbContext.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Count != 0).ToList();

        return domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();
    }

    public static void ClearAllDomainEvents(DbContext DbContext)
    {
        var domainEntities = DbContext.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Count != 0).ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());
    }
}