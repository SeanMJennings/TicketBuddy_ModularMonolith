using Domain.Notifications;

namespace Application.Notifications;

public class GetNotifications(IPersistNotifications repository)
{
    public async Task<IReadOnlyList<Notification>> Execute(Guid userId) => await repository.GetByUserId(userId);
}