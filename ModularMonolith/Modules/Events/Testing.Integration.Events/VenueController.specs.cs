using NUnit.Framework;

namespace Integration;

public partial class VenueControllerSpecs
{
    [Test]
    public async Task can_create_venue()
    {
              Given(a_request_to_create_a_venue);
        await When(creating_the_venue);
        await And(requesting_the_venue);
              Then(the_venue_is_created);
    }

    [Test]
    public async Task can_list_venues()
    {
        await Given(a_venue_exists);
        await And(another_venue_exists);
        await And(a_third_venue_exists);
        await When(listing_the_venues);
              Then(the_venues_are_listed);
    }
}