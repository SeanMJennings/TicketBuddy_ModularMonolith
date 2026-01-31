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

    [Test]
    public async Task cannot_mark_non_existent_notification_as_read()
    {
        await When(Validating(marking_notification_as_read));
        Then(Informs($"Notification with id {notificationId} was not found."));
        And(an_entity_not_found_exception_was_thrown);
    }
}