using Domain.Notifications;

namespace Controllers.Notifications;

public record NotificationResponse(
    Guid Id,
    Guid UserId,
    NotificationType Type,
    string Payload,
    bool IsRead,
    DateTimeOffset CreatedAt);