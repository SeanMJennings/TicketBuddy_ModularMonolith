using Domain.Exceptions;
using Domain.Notifications;

namespace Application.Notifications;

public class MarkNotificationAsRead(IPersistNotifications repository, INotificationsUnitOfWork unitOfWork)
{
    public async Task Execute(Guid notificationId)
    {
        var notification = await repository.GetById(notificationId);
        if (notification is null) throw new EntityNotFoundException(nameof(Notification), notificationId);

        notification.MarkAsRead();
        await repository.Update(notification);
        await unitOfWork.Commit();
    }
}