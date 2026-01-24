using NUnit.Framework;

namespace Integration.Venues;

public partial class CreateVenueSpecs
{
    [Test]
    public async Task can_create_venue()
    {
              Given(a_request_to_create_a_venue);
        await When(creating_the_venue);
        await And(requesting_the_venue);
              Then(the_venue_is_created);
    }
}