using Application.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Notifications;

[ApiController]
public class MarkNotificationAsReadEndpoint(MarkNotificationAsRead markNotificationAsRead) : ControllerBase
{
    [HttpPost(Routes.NotificationRead)]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        await markNotificationAsRead.Execute(id);
        return NoContent();
    }
}