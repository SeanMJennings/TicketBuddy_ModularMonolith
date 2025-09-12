using Domain.Contracts;
using Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Commands;

public abstract class UnitOfWorkDbContext<T>(DbContextOptions<T> options, DomainEventsDispatcher domainEventsDispatcher) 
    : DbContext(options), IAmAUnitOfWork 
    where T : DbContext
{
    public async Task<int> Commit(CancellationToken cancellationToken = default)
    {
        await domainEventsDispatcher.DispatchEvents(this);
        return await SaveChangesAsync(cancellationToken);
    }
}