using Domain.Notifications;
using Infrastructure.Notifications.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Notifications.Notification;

public class NotificationRepository(NotificationDbContext dbContext) : IPersistNotifications
{
    public async Task Add(Domain.Notifications.Notification notification)
    {
        await dbContext.Notifications.AddAsync(notification);
    }

    public async Task<Domain.Notifications.Notification?> GetById(Guid id)
    {
        return await dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IReadOnlyList<Domain.Notifications.Notification>> GetByUserId(Guid userId)
    {
        return await dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public Task Update(Domain.Notifications.Notification notification)
    {
        dbContext.Entry(notification).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}