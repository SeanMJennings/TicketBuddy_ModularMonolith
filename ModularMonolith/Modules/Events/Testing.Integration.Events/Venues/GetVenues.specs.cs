using NUnit.Framework;

namespace Integration.Venues;

public partial class GetVenuesSpecs
{
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