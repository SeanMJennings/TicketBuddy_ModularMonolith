using NUnit.Framework;

namespace Component.Api;

public partial class EventApiSpecs
{
    [Test]
    public async Task can_create_event()
    {
              Given(a_request_to_create_an_event);
        await When(creating_the_event);
        await And(requesting_the_event);
        await Then(the_event_is_created);
              And(an_integration_event_is_published);
    }

    [Test]
    public async Task can_update_event()
    {
        await Given(an_event_exists);
              And(a_request_to_update_the_event);
        await When(updating_the_event);
        await And(requesting_the_updated_event);
        await Then(the_event_is_updated);
              And(an_another_integration_event_is_published);
    }
    
    [Test]
    public async Task can_list_events()
    {
        await Given(an_event_exists);
        await And(another_event_exists);
        await And(a_third_event_exists);
        await When(listing_the_events);
        await Then(the_events_are_listed_earliest_first);
    }
}