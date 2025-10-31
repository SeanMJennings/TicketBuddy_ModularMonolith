using NUnit.Framework;

namespace Acceptance;

public partial class TicketBuddySpecs
{
    [Test]
    public async Task user_can_purchase_tickets_for_an_event()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(tickets_are_available_for_the_event);
        await When(the_user_purchases_tickets_for_the_event);
        await Then(the_purchase_is_successful);
    }
}