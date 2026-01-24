using NUnit.Framework;

namespace Integration.Consumer;

public partial class TicketPurchasedConsumerSpecs
{
    [Test]
    public async Task creates_notification_when_ticket_is_purchased()
    {
        await Given(a_ticket_purchased_message);
        await When(the_consumer_processes_the_message);
        await Then(a_notification_is_created_for_the_user);
    }
}