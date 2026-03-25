using NUnit.Framework;

namespace Integration.Venues;

public partial class GetVenuesByIdSpecs
{
    [Test]
    public async Task get_venue_returns_the_venue_when_it_exists()
    {
        await Given(a_venue_exists);
        await When(requesting_the_venue);
              Then(the_venue_is_returned);
    }

    [Test]
    public async Task get_venue_returns_not_found_for_non_existent_venue()
    {
        await When(requesting_a_non_existent_venue);
              Then(a_not_found_response_is_returned);
    }
}