using Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Commands;

public abstract class UnitOfWorkDbContext<T>(DbContextOptions<T> options, DomainEventsDispatcher domainEventsDispatcher) 
    : DbContext(options) 
    where T : DbContext
{
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        await domainEventsDispatcher.DispatchEvents(this);
    }
}