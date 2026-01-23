using NUnit.Framework;

namespace Unit;

public partial class NotificationSpecs
{
    [Test]
    public void can_create_a_notification()
    {
        Given(valid_inputs);
        When(creating_a_notification);
        Then(the_notification_is_created);
    }

    [Test]
    public void notification_must_have_a_user_id()
    {
        Given(valid_inputs);
        And(an_empty_user_id);
        When(Validating(creating_a_notification));
        Then(Informs("Entity ID cannot be an empty GUID"));
    }


    [Test]
    public void notification_is_unread_by_default()
    {
        Given(valid_inputs);
        When(creating_a_notification);
        Then(the_notification_is_unread);
    }

    [Test]
    public void can_mark_notification_as_read()
    {
        Given(a_valid_notification);
        When(marking_as_read);
        Then(the_notification_is_read);
    }
}
