using NUnit.Framework;

namespace Component.Api;

public partial class TicketApiSpecs
{
    [Test]
    public void can_release_tickets()
    {
        Given(an_event_exists);
        When(requesting_the_tickets);
        Then(the_tickets_are_released);
    }
    
    [Test]
    public void user_can_purchase_two_tickets()
    {
        Given(an_event_exists);
        And(a_user_exists);
        And(requesting_the_tickets);
        When(purchasing_two_tickets);
        Then(the_tickets_are_purchased);
    }
    
    [Test]
    public void can_update_ticket_price_for_unpurchased_tickets()
    {
        Given(an_event_exists);
        And(a_user_exists);
        And(requesting_the_tickets);
        And(two_tickets_are_purchased);
        When(updating_the_ticket_prices);
        Then(the_ticket_prices_are_updated);
        And(purchased_tickets_are_not_updated);
    }
    
    [Test]
    public void user_cannot_purchase_tickets_that_are_purchased()
    {
        Given(an_event_exists);
        And(a_user_exists);
        And(requesting_the_tickets);
        And(two_tickets_are_purchased);
        When(purchasing_two_tickets_again);
        Then(user_informed_they_cannot_purchase_tickets_that_are_purchased);
    }
    
    [Test]
    public void cannot_purchase_tickets_that_do_not_exist()
    {
        Given(an_event_exists);
        And(a_user_exists);
        When(purchasing_two_non_existent_tickets);
        Then(user_informed_they_cannot_purchase_tickets_that_are_non_existent);
    }
    
    [Test]
    public void user_can_reserve_a_ticket_for_15_minutes()
    {
        Given(an_event_exists);
        And(a_user_exists);
        And(requesting_the_tickets);
        When(reserving_a_ticket);
        Then(the_ticket_is_reserved);
        And(the_reservation_expires_in_15_minutes);
    }
    
    [Test]
    public void another_user_cannot_reserve_a_ticket_that_is_already_reserved()
    {
        Given(an_event_exists);
        And(a_user_exists);
        And(requesting_the_tickets);
        And(reserving_a_ticket);
        When(another_user_reserving_a_ticket);
        Then(user_informed_they_cannot_reserve_an_already_reserved_ticket);
    }
    
    [Test]
    public void different_user_cannot_purchase_a_reserved_ticket()
    {
        Given(an_event_exists);
        And(a_user_exists);
        And(another_user_exists);
        And(requesting_the_tickets);
        And(reserving_a_ticket);
        When(another_user_purchasing_the_reserved_ticket);
        Then(another_user_informed_they_cannot_purchase_a_reserved_ticket);
    }
    
    [Test]
    public void a_user_can_purchase_their_own_reserved_ticket()
    {
        Given(an_event_exists);
        And(a_user_exists);
        And(requesting_the_tickets);
        And(reserving_a_ticket);
        When(the_user_purchases_their_reserved_ticket);
        Then(the_tickets_are_purchased);
    }
    
    [Test]
    public void same_user_can_extend_their_own_reservation()
    {
        Given(an_event_exists);
        And(a_user_exists);
        And(requesting_the_tickets);
        And(reserving_a_ticket);
        When(the_user_extends_their_reservation);
        Then(the_ticket_is_reserved);
    }
    
    [Test]
    public void event_sold_out_integration_event_fires_when_all_tickets_are_purchased()
    {
        Given(an_event_exists);
        And(a_user_exists);
        And(requesting_the_tickets);
        When(purchasing_all_tickets);
        Then(event_sold_out_integration_event_is_published);
    } 
}