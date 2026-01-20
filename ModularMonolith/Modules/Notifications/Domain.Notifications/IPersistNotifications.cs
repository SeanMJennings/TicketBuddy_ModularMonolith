namespace Domain.Notifications;

public interface IPersistNotifications
{
    Task Add(Notification notification);
    Task<Notification?> GetById(Guid id);
    Task<IReadOnlyList<Notification>> GetByUserId(Guid userId);
    Task Update(Notification notification);
}