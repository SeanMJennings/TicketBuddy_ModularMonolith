using NUnit.Framework;

namespace Integration.Tickets;

public partial class GetTicketsForEventSpecs
{
    [Test]
    public async Task can_release_tickets()
    {
        await Given(an_event_exists);
        await When(requesting_the_tickets);
        await Then(the_tickets_are_released);
    }

    [Test]
    public async Task cannot_get_tickets_for_non_existent_event()
    {
        await When(Validating(requesting_the_tickets));
        Then(Informs($"Event with id {event_id} was not found."));
        And(an_entity_not_found_exception_was_thrown);
    }
}