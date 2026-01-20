using Domain.Notifications;

namespace Infrastructure.Notifications.Core;

public class UnitOfWork(NotificationDbContext dbContext) : INotificationsUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await dbContext.Commit(cancellationToken);
    }
}