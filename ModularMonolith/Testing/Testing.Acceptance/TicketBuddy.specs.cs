using NUnit.Framework;

namespace Acceptance;

public partial class TicketBuddySpecs
{
    [Test]
    public void user_can_purchase_tickets_for_an_event()
    {
        Given(an_event_exists);
        And(a_user_exists);
        And(tickets_are_available_for_the_event);
        When(the_user_purchases_tickets_for_the_event);
        Then(the_purchase_is_successful);
    }
}