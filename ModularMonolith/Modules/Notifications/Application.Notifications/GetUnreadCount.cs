using Domain.Notifications;

namespace Application.Notifications;

public class GetUnreadCount(IPersistNotifications repository)
{
    public async Task<int> Execute(Guid userId) => await repository.GetUnreadCountByUserId(userId);
}