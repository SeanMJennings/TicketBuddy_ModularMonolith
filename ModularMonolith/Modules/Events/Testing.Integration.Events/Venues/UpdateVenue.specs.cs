using NUnit.Framework;

namespace Integration.Venues;

public partial class UpdateVenueSpecs
{
    [Test]
    public async Task can_update_venue()
    {
        await Given(a_venue_exists);
              And(a_request_to_update_the_venue);
        await When(updating_the_venue);
        await And(requesting_the_updated_venue);
              Then(the_venue_is_updated);
    }

    [Test]
    public async Task cannot_update_venue_to_duplicate_address()
    {
        await Given(a_venue_exists);
        await And(another_venue_exists);
              And(a_request_to_update_venue_to_duplicate_address);
        await When(Validating(updating_the_venue));
              Then(Informs("A venue already exists at this address"));
    }

    [Test]
    public async Task venue_upserted_message_published_on_update()
    {
        await Given(a_venue_exists);
              And(a_request_to_update_the_venue);
        await When(updating_the_venue);
              Then(a_venue_upserted_message_is_published);
    }
}