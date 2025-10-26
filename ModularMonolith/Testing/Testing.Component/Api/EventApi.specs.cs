using NUnit.Framework;

namespace Component.Api;

public partial class EventApiSpecs
{
    [Test]
    public void can_create_event()
    {
        Given(a_request_to_create_an_event);
        When(creating_the_event);
        And(requesting_the_event);
        Then(the_event_is_created);
        And(an_integration_event_is_published);
    }

    [Test]
    public void can_update_event()
    {
        Given(an_event_exists);
        And(a_request_to_update_the_event);
        When(updating_the_event);
        And(requesting_the_updated_event);
        Then(the_event_is_updated);
        And(an_another_integration_event_is_published);
    }
    
    [Test]
    public void can_list_events()
    {
        Given(an_event_exists);
        And(another_event_exists);
        And(a_third_event_exists);
        When(listing_the_events);
        Then(the_events_are_listed_earliest_first);
    }
}