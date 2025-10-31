using NUnit.Framework;

namespace Integration;

public partial class EventControllerSpecs
{
    [Test]
    public async Task can_create_event()
    {
              Given(a_request_to_create_an_event);
        await When(creating_the_event);
        await And(requesting_the_event);
              Then(the_event_is_created);
              And(an_integration_event_is_published);
    }
    
    [Test]
    public async Task cannot_create_event_with_date_in_the_past()
    {
              Given(a_request_to_create_an_event_with_a_date_in_the_past);
        await When(creating_the_event_that_will_fail);
              Then(the_event_is_not_created);
    }
    
    [Test]
    public async Task cannot_double_book_venue()
    {
        await Given(an_event_exists);
              And(a_request_to_create_an_event_with_the_same_venue_and_time);
        await When(creating_the_event_that_will_fail);
              Then(the_user_is_informed_that_the_venue_is_unavailable);
    }
    
    [Test]
    public async Task can_update_event()
    {
        await Given(an_event_exists);
              And(a_request_to_update_the_event);
        await When(updating_the_event);
        await And(requesting_the_updated_event);
              Then(the_event_is_updated);
              And(an_another_integration_event_is_published);
    }
    
    [Test]
    public async Task cannot_update_and_double_book_venue()
    {
        await Given(an_event_exists);
        await And(another_event_at_same_venue_exists);
              And(a_request_to_update_the_event_with_a_venue_and_time_that_will_double_book);
        await When(updating_the_event_that_will_fail);
              Then(the_user_is_informed_that_the_venue_is_unavailable);
    }
    
    [Test]
    public async Task cannot_update_event_with_start_date_in_the_past()
    {
        await Given(an_event_exists);
              And(a_request_to_update_the_event_with_a_date_in_the_past);
        await When(updating_the_event_that_will_fail);
              Then(the_event_is_not_updated);
    }
    
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

    [Test]
    public async Task can_mark_events_as_sold_out()
    {
        await Given(an_event_exists);
              And(it_has_sold_out);
        await When(requesting_the_event);
              Then(the_event_is_marked_as_sold_out);
    }
}