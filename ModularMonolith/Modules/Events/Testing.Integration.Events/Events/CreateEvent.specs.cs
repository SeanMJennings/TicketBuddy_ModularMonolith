using NUnit.Framework;

namespace Integration.Events;

public partial class CreateEventSpecs
{
    [Test]
    public async Task can_create_event()
    {
              Given(a_request_to_create_an_event);
        await When(creating_the_event);
        await And(requesting_the_event);
              Then(the_event_is_created);
    }

    [Test]
    public async Task cannot_create_event_with_date_in_the_past()
    {
              Given(a_request_to_create_an_event_with_a_date_in_the_past);
        await When(Validating(creating_the_event_that_will_fail));
              Then(Informs("Event date cannot be in the past"));
    }

    [Test]
    public async Task cannot_double_book_venue()
    {
        await Given(an_event_exists);
              And(a_request_to_create_an_event_with_the_same_venue_and_time);
        await When(Validating(creating_the_event_that_will_fail));
              Then(Informs("Venue is not available at the selected time"));
    }

    [Test]
    public async Task creating_an_event_persists_outbox_message_atomically_to_the_database()
    {
              Given(a_request_to_create_an_event);
        await When(creating_the_event);
        await Then(outbox_message_is_persisted_to_the_event_schema);
    }
}