using Domain.Entities;

namespace Domain.Notifications;

public class Notification : Entity, IAmAnAggregateRoot
{
    internal Notification(
        Guid id,
        Guid userId,
        NotificationType type,
        string payload,
        DateTimeOffset createdAt) : base(id)
    {
        Validation.BasedOn(errors =>
        {
            if (userId == Guid.Empty)
                errors.Add("Entity ID cannot be an empty GUID");
            if (!Enum.IsDefined(typeof(NotificationType), type))
                errors.Add("NotificationType cannot be null or empty");
        });

        UserId = userId;
        Type = type;
        Payload = payload;
        IsRead = false;
        CreatedAt = createdAt;
    }

    public static Notification Create(
        Guid id,
        Guid userId,
        NotificationType type,
        string payload,
        DateTimeOffset createdAt)
    {
        return new Notification(id, userId, type, payload, createdAt);
    }

    public Guid UserId { get; }
    public NotificationType Type { get; }
    public string Payload { get; }
    public bool IsRead { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public void MarkAsRead() => IsRead = true;
}