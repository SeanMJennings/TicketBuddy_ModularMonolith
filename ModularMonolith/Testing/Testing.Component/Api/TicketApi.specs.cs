using NUnit.Framework;

namespace Component.Api;

public partial class TicketApiSpecs
{
    [Test]
    public async Task can_release_tickets()
    {
        await Given(an_event_exists);
        await When(requesting_the_tickets);
        await Then(the_tickets_are_released);
    }
    
    [Test]
    public async Task user_can_purchase_two_tickets()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(requesting_the_tickets);
        await When(purchasing_two_tickets);
        await Then(the_tickets_are_purchased);
    }
    
    [Test]
    public async Task can_update_ticket_price_for_unpurchased_tickets()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(requesting_the_tickets);
        await And(two_tickets_are_purchased);
        await When(updating_the_ticket_prices);
        await Then(the_ticket_prices_are_updated);
        await And(purchased_tickets_are_not_updated);
    }

    [Test]
    public async Task user_can_reserve_a_ticket_for_15_minutes()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(requesting_the_tickets);
        await When(reserving_a_ticket);
        await Then(the_ticket_is_reserved);
              And(the_reservation_expires_in_15_minutes);
    }
}