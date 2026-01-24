using NUnit.Framework;

namespace Integration.Tickets;

public partial class PurchaseTicketsSpecs
{
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
    public async Task user_cannot_purchase_tickets_that_are_purchased()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(requesting_the_tickets);
        await And(two_tickets_are_purchased);
        await When(purchasing_two_tickets_again);
              Then(user_informed_they_cannot_purchase_tickets_that_are_purchased);
    }

    [Test]
    public async Task cannot_purchase_tickets_that_do_not_exist()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await When(purchasing_two_non_existent_tickets);
              Then(user_informed_they_cannot_purchase_tickets_that_are_non_existent);
    }

    [Test]
    public async Task cannot_purchase_tickets_for_non_existent_event()
    {
        await Given(a_user_exists);
        await When(Validating(purchasing_tickets_for_non_existent_event));
              Then(Informs($"Event with id {nonExistentEventId} not found"));
    }

    [Test]
    public async Task different_user_cannot_purchase_a_reserved_ticket()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(another_user_exists);
        await And(requesting_the_tickets);
        await And(reserving_a_ticket);
        await When(another_user_purchasing_the_reserved_ticket);
              Then(another_user_informed_they_cannot_purchase_a_reserved_ticket);
    }

    [Test]
    public async Task a_user_can_purchase_their_own_reserved_ticket()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(requesting_the_tickets);
        await And(reserving_a_ticket);
        await When(the_user_purchases_their_reserved_ticket);
        await Then(the_tickets_are_purchased);
    }

    [Test]
    public async Task event_sold_out_integration_event_fires_when_all_tickets_are_purchased()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(requesting_the_tickets);
        await When(purchasing_all_tickets);
        await Then(event_sold_out_integration_event_is_published);
    }
}