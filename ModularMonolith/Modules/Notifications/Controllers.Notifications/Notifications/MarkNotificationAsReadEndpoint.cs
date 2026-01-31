using Application;
using Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Notifications;

[ApiController]
public class MarkNotificationAsReadEndpoint(MarkNotificationAsRead markNotificationAsRead) : ControllerBase
{
    [HttpPost(Routes.NotificationRead)]
    [Authorize(Roles = UserRoles.Customer)]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        await markNotificationAsRead.Execute(id);
        return NoContent();
    }
}