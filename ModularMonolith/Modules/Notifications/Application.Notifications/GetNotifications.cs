using Domain.Notifications;

namespace Application.Notifications;

public class GetNotifications(IPersistNotifications repository)
{
    public async Task<IReadOnlyList<Notification>> Execute(Guid userId)
    {
        var notifications = await repository.GetByUserId(userId);
        return notifications.OrderByDescending(n => n.CreatedAt).ToList();
    }
}