using NUnit.Framework;

namespace Component.Api;

public partial class EventApiSpecs
{
    [Test]
    public async Task can_create_event()
    {
              Given(an_admin_user_exists);
              And(a_request_to_create_an_event);
        await When(creating_the_event);
        await And(requesting_the_event);
        await Then(the_event_is_created);
              And(an_integration_event_is_published);
    }
    
    [Test]
    public async Task a_non_admin_cannot_create_event()
    {
              Given(an_admin_user_exists);
              And(a_request_to_create_an_event_as_a_non_admin_user);
        await When(creating_the_event_that_should_fail);
              Then(the_event_creation_is_forbidden);
    }
    
    [Test]
    public async Task an_anonymous_user_can_view_events()
    {
              Given(an_admin_user_exists);
        await And(an_event_exists);
        await When(listing_the_events_as_an_anonymous_user);
        await Then(the_events_are_returned);
    }

    [Test]
    public async Task can_update_event()
    {
              Given(an_admin_user_exists);
        await And(an_event_exists);
              And(a_request_to_update_the_event);
        await When(updating_the_event);
        await And(requesting_the_updated_event);
        await Then(the_event_is_updated);
              And(an_another_integration_event_is_published);
    }
    
    [Test]
    public async Task cannot_update_non_existent_event()
    {
              Given(an_admin_user_exists);
              And(a_request_to_update_the_event);
        await When(Validating(updating_the_non_existent_event));
        await Then(returns_a_not_found_response);
    }
    
    [Test]
    public async Task can_list_events()
    {
              Given(an_admin_user_exists);
        await And(an_event_exists);
        await And(another_event_exists);
        await And(a_third_event_exists);
        await When(listing_the_events);
        await Then(the_events_are_listed_earliest_first);
    }

    [Test]
    public async Task can_create_venue()
    {
              Given(an_admin_user_exists);
              And(a_request_to_create_a_venue);
        await When(creating_the_venue);
        await And(requesting_the_venue);
        await Then(the_venue_is_created);
              And(a_venue_integration_event_is_published);
    }

    [Test]
    public async Task a_non_admin_cannot_create_venue()
    {
              Given(an_admin_user_exists);
              And(a_request_to_create_a_venue_as_a_non_admin_user);
        await When(creating_the_venue_that_should_fail);
              Then(the_venue_creation_is_forbidden);
    }

    [Test]
    public async Task an_anonymous_user_can_view_venues()
    {
              Given(an_admin_user_exists);
              And(a_venue_exists);
        await When(listing_the_venues_as_an_anonymous_user);
        await Then(the_venues_are_returned);
    }

    [Test]
    public async Task an_anonymous_user_can_view_venue_by_id()
    {
              Given(an_admin_user_exists);
              And(a_venue_exists);
        await When(requesting_the_venue_as_an_anonymous_user);
        await Then(the_venue_is_returned);
    }

    [Test]
    public async Task can_update_venue()
    {
              Given(an_admin_user_exists);
              And(a_venue_exists);
              And(a_request_to_update_the_venue);
        await When(updating_the_venue);
        await And(requesting_the_updated_venue);
        await Then(the_venue_is_updated);
    }

    [Test]
    public async Task a_non_admin_cannot_update_venue()
    {
              Given(an_admin_user_exists);
              And(a_venue_exists);
              And(a_request_to_update_the_venue_as_a_non_admin_user);
        await When(updating_the_venue_that_should_fail);
              Then(the_venue_update_is_forbidden);
    }
}