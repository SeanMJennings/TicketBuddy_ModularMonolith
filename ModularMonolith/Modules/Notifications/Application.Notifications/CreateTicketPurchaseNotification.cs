using Domain.Notifications;

namespace Application.Notifications;

public class CreateTicketPurchaseNotification(
    IPersistNotifications repository,
    INotificationsUnitOfWork unitOfWork)
{
    public async Task Execute(Guid userId, Guid ticketId, Guid eventId, string eventName)
    {
        var notification = Notification.Create(
            Guid.NewGuid(),
            userId,
            "TicketPurchased",
            $"{{\"ticketId\":\"{ticketId}\",\"eventId\":\"{eventId}\",\"eventName\":\"{eventName}\"}}",
            DateTimeOffset.UtcNow);

        await repository.Add(notification);
        await unitOfWork.Commit();
    }
}