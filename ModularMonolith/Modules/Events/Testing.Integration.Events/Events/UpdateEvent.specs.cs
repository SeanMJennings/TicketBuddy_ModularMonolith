using NUnit.Framework;

namespace Integration.Events;

public partial class UpdateEventSpecs
{
    [Test]
    public async Task can_update_event()
    {
        await Given(an_event_exists);
              And(a_request_to_update_the_event);
        await When(updating_the_event);
        await And(requesting_the_updated_event);
              Then(the_event_is_updated);
        await And(an_another_integration_event_is_published);
    }

    [Test]
    public async Task cannot_update_and_double_book_venue()
    {
        await Given(an_event_exists);
        await And(another_event_at_same_venue_exists);
              And(a_request_to_update_the_event_with_a_venue_and_time_that_will_double_book);
        await When(Validating(updating_the_event_that_will_fail));
              Then(Informs("Venue is not available at the selected time"));
    }

    [Test]
    public async Task cannot_update_event_with_start_date_in_the_past()
    {
        await Given(an_event_exists);
              And(a_request_to_update_the_event_with_a_date_in_the_past);
        await When(Validating(updating_the_event_that_will_fail));
              Then(Informs("Event date cannot be in the past"));
    }
    
    [Test]
    public async Task cannot_update_non_existent_event()
    {
          Given(a_request_to_update_the_event);
          await When(Validating(updating_the_event));
          Then(Informs($"Event with id {returned_id} was not found."));
          And(an_entity_not_found_exception_was_thrown);
    }
}