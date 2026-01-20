using Application.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Notifications;

[ApiController]
public class GetNotificationsEndpoint(GetNotifications getNotifications) : ControllerBase
{
    [HttpGet("/notifications")]
    public async Task<IEnumerable<NotificationResponse>> GetNotifications()
    {
        var userId = User.GetUserId();
        var notifications = await getNotifications.Execute(userId);
        return notifications.Select(n => new NotificationResponse(
            n.Id,
            n.UserId,
            n.Type,
            n.Payload,
            n.IsRead,
            n.CreatedAt));
    }
}