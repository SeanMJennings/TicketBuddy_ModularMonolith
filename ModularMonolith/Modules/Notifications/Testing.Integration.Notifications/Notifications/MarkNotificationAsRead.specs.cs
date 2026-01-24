using NUnit.Framework;

namespace Integration.Notifications;

public partial class MarkNotificationAsReadSpecs
{
    [Test]
    public async Task can_mark_notification_as_read()
    {
        await Given(an_unread_notification_exists);
        await When(marking_notification_as_read);
        await Then(the_notification_is_marked_as_read);
    }
}