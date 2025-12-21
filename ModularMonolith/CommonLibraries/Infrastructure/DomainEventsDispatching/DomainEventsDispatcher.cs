using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DomainEventsDispatching;

public class DomainEventsDispatcher(DomainEventsMapper domainEventsMapper)
    {
        public async Task DispatchEvents(DbContext dbContext)
        {
            var domainEvents = DomainEventsAccessor.GetAllDomainEvents(dbContext);
            
            DomainEventsAccessor.ClearAllDomainEvents(dbContext);

            foreach (var domainEvent in domainEvents)
            {
                var handler = domainEventsMapper.GetHandler(domainEvent);
                await handler.Handle(domainEvent);
            }
        }
    }