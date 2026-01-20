using BDD;
using Domain.Notifications;
using Shouldly;

namespace Unit;

public partial class NotificationSpecs : Specification
{
    private Guid id;
    private Guid userId;
    private string type = null!;
    private string payload = null!;
    private DateTimeOffset createdAt;
    private Notification theNotification = null!;

    private const string validType = "TicketPurchased";
    private const string validPayload = """{"ticketId":"12345","eventName":"Concert"}""";

    protected override void before_each()
    {
        base.before_each();
        id = Guid.NewGuid();
        userId = Guid.NewGuid();
        type = null!;
        payload = null!;
        createdAt = DateTimeOffset.UtcNow;
        theNotification = null!;
    }

    private void valid_inputs()
    {
        type = validType;
        payload = validPayload;
    }

    private void an_empty_user_id()
    {
        userId = Guid.Empty;
    }

    private void an_empty_type()
    {
        type = string.Empty;
    }

    private void a_valid_notification()
    {
        valid_inputs();
        creating_a_notification();
    }

    private void creating_a_notification()
    {
        var notificationType = new NotificationType(type);
        theNotification = Notification.Create(id, userId, notificationType, payload, createdAt);
    }

    private void marking_as_read()
    {
        theNotification.MarkAsRead();
    }

    private void the_notification_is_created()
    {
        theNotification.ShouldNotBeNull();
        theNotification.Id.ShouldBe(id);
        theNotification.UserId.ShouldBe(userId);
        theNotification.Type.ToString().ShouldBe(validType);
        theNotification.Payload.ShouldBe(validPayload);
        theNotification.CreatedAt.ShouldBe(createdAt);
    }

    private void the_notification_is_unread()
    {
        theNotification.IsRead.ShouldBeFalse();
    }

    private void the_notification_is_read()
    {
        theNotification.IsRead.ShouldBeTrue();
    }
}
