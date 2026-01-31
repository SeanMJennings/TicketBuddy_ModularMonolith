using NUnit.Framework;

namespace Integration.Tickets;

public partial class ReserveTicketsSpecs
{
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

    [Test]
    public async Task another_user_cannot_reserve_a_ticket_that_is_already_reserved()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(requesting_the_tickets);
        await And(reserving_a_ticket);
        await When(Validating(another_user_reserving_a_ticket));
              Then(Informs("Ticket already reserved"));
    }

    [Test]
    public async Task same_user_can_extend_their_own_reservation()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(requesting_the_tickets);
        await And(reserving_a_ticket);
        await When(the_user_extends_their_reservation);
        await Then(the_ticket_is_reserved);
    }
}