using NUnit.Framework;

namespace Component.Api;

public partial class NotificationApiSpecs
{
    [Test]
    public async Task anonymous_user_cannot_get_notifications()
    {
        await When(requesting_notifications_as_an_anonymous_user);
              Then(the_request_is_unauthorized);
    }

    [Test]
    public async Task anonymous_user_cannot_mark_notification_as_read()
    {
        await When(marking_notification_as_read_as_an_anonymous_user);
              Then(the_request_is_unauthorized);
    }

    [Test]
    public async Task anonymous_user_cannot_get_unread_count()
    {
        await When(requesting_unread_count_as_an_anonymous_user);
              Then(the_request_is_unauthorized);
    }

    [Test]
    public async Task authenticated_user_can_get_their_notifications()
    {
        await Given(a_notification_exists_for_the_user);
        await When(requesting_notifications);
              Then(the_notifications_are_returned);
    }

    [Test]
    public async Task authenticated_user_can_mark_their_notification_as_read()
    {
        await Given(a_notification_exists_for_the_user);
        await When(marking_notification_as_read);
              Then(the_request_succeeds);
    }

    [Test]
    public async Task authenticated_user_can_get_their_unread_count()
    {
        await Given(a_notification_exists_for_the_user);
        await When(requesting_unread_count);
              Then(the_unread_count_is_returned);
    }
}