using NUnit.Framework;

namespace Integration.Api;

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
    public void cannot_create_event_with_date_in_the_past()
    {
        Given(a_request_to_create_an_event_with_a_date_in_the_past);
        When(creating_the_event_that_will_fail);
        Then(the_event_is_not_created);
    }
    
    [Test]
    public void cannot_double_book_venue()
    {
        Given(an_event_exists);
        And(a_request_to_create_an_event_with_the_same_venue_and_time);
        When(creating_the_event_that_will_fail);
        Then(the_user_is_informed_that_the_venue_is_unavailable);
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
    public void cannot_update_and_double_book_venue()
    {
        Given(an_event_exists);
        And(another_event_at_same_venue_exists);
        And(a_request_to_update_the_event_with_a_venue_and_time_that_will_double_book);
        When(updating_the_event_that_will_fail);
        Then(the_user_is_informed_that_the_venue_is_unavailable);
    }
    
    [Test]
    public void cannot_update_event_with_start_date_in_the_past()
    {
        Given(an_event_exists);
        And(a_request_to_update_the_event_with_a_date_in_the_past);
        When(updating_the_event_that_will_fail);
        Then(the_event_is_not_updated);
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
    
    [Test]
    public void does_not_list_events_in_the_past()
    {
        Given(an_imminent_event_exists);
        And(an_event_exists);
        And(a_short_wait);
        When(listing_the_events);
        Then(the_events_are_listed_without_the_past_event);
    }

    [Test]
    public void can_mark_events_as_sold_out()
    {
        Given(an_event_exists);
        And(it_has_sold_out);
        When(requesting_the_event);
        Then(the_event_is_marked_as_sold_out);
    }
}