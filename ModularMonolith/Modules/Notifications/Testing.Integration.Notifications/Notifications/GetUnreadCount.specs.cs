using NUnit.Framework;

namespace Integration.Notifications;

public partial class GetUnreadCountSpecs
{
    [Test]
    public async Task can_get_unread_notification_count()
    {
        await Given(unread_and_read_notifications_exist);
        await When(requesting_unread_count);
              Then(unread_count_is_returned);
    }
}