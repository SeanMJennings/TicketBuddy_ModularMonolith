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
}