using NUnit.Framework;

namespace Unit;

public partial class TicketSpecs
{
    [Test]
    public void can_create_a_ticket()
    {
        Given(valid_ticket_inputs);
        When(creating_a_ticket);
        Then(the_ticket_is_created);
    }

    [Test]
    public void can_purchase_an_available_ticket()
    {
        Given(a_valid_ticket);
        When(purchasing_the_ticket);
        Then(the_ticket_is_purchased);
    }

    [Test]
    public void cannot_purchase_an_already_purchased_ticket()
    {
        Given(a_purchased_ticket);
        When(Validating(purchasing_the_ticket));
        Then(Informs("Tickets are not available"));
    }

    [Test]
    public void can_update_price_on_available_ticket()
    {
        Given(a_valid_ticket);
        When(updating_the_ticket_price);
        Then(the_ticket_price_is_updated);
    }

    [Test]
    public void cannot_update_price_on_purchased_ticket()
    {
        Given(a_purchased_ticket);
        When(updating_the_ticket_price);
        Then(the_ticket_price_is_not_updated);
    }
}

