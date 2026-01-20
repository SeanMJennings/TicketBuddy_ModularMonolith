namespace Controllers.Notifications;

public record NotificationResponse(
    Guid Id,
    Guid UserId,
    string Type,
    string Payload,
    bool IsRead,
    DateTimeOffset CreatedAt);