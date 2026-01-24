using NUnit.Framework;

namespace Integration.Events;

public partial class EventSoldOutConsumerSpecs
{
    [Test]
    public async Task can_mark_events_as_sold_out()
    {
        await Given(an_event_exists);
              And(it_has_sold_out);
        await When(requesting_the_event);
              Then(the_event_is_marked_as_sold_out);
    }

    [Test]
    public async Task marking_non_existent_event_as_sold_out_does_nothing()
    {
        await When(marking_non_existent_event_as_sold_out);
    }
}