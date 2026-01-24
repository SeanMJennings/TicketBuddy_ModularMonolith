using NUnit.Framework;

namespace Integration.Events;

public partial class GetEventsSpecs
{
    [Test]
    public async Task can_list_events()
    {
        await Given(an_event_exists);
        await And(another_event_exists);
        await And(a_third_event_exists);
        await When(listing_the_events);
              Then(the_events_are_listed_earliest_first);
    }

    [Test]
    public async Task does_not_list_events_in_the_past()
    {
        await Given(an_imminent_event_exists);
        await And(an_event_exists);
              And(a_short_wait);
        await When(listing_the_events);
              Then(the_events_are_listed_without_the_past_event);
    }
}