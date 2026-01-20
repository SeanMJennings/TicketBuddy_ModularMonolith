using NUnit.Framework;

namespace Integration;

public partial class NotificationControllerSpecs
{
    [Test]
    public async Task can_get_notifications_for_user()
    {
        await Given(notifications_exist_for_user);
        await When(requesting_notifications);
              Then(notifications_are_returned_ordered_by_created_at_descending);
    }

    [Test]
    public async Task can_mark_notification_as_read()
    {
        await Given(an_unread_notification_exists);
        await When(marking_notification_as_read);
        await Then(the_notification_is_marked_as_read);
    }
}