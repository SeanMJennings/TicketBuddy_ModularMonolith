using Domain.Events.Contracts;

namespace Infrastructure.Events.Core;

public class UnitOfWork(EventDbContext eventDbContext) : IEventsUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await eventDbContext.Commit(cancellationToken);
    }
}

