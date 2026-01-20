using Application.Authentication;
using Application.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Notifications;

[ApiController]
public class GetUnreadCountEndpoint(GetUnreadCount getUnreadCount) : ControllerBase
{
    [HttpGet(Routes.UnreadCount)]
    public async Task<int> GetUnreadCount()
    {
        var userId = User.GetUserId();
        return await getUnreadCount.Execute(userId);
    }
}