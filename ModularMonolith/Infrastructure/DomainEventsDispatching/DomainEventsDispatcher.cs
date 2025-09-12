using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DomainEventsDispatching;

public class DomainEventsDispatcher(
    DomainEventsAccessor DomainEventsAccessor,
    DomainEventsMapper DomainEventsMapper)
    {
        public async Task DispatchEvents(DbContext dbContext)
        {
            var domainEvents = DomainEventsAccessor.GetAllDomainEvents(dbContext);
            
            DomainEventsAccessor.ClearAllDomainEvents(dbContext);

            foreach (var domainEvent in domainEvents)
            {
                var handler = DomainEventsMapper.GetHandler(domainEvent);
                await handler.Handle(domainEvent);
            }
        }
    }