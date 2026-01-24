using Controllers.Events.Requests;
using Controllers.Events.Venue;
using Domain.Events.Venue;
using Infrastructure.Configuration;
using Infrastructure.Events.Core.Configuration;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testing;

namespace Integration.Venues;

public partial class CreateVenueSpecs : TruncateDbSpecification
{
    private CreateVenueEndpoint createVenueEndpoint = null!;
    private GetVenueByIdEndpoint getVenueByIdEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private VenuePayload venuePayload = null!;
    private Guid returned_id;
    private Domain.Events.Venue.Venue theVenue = null!;
    private const string venueName = "The Apollo";
    private const string street = "123 Main Street";
    private const string city = "Manchester";
    private const string postcode = "M1 1AA";
    private const uint capacity = 25;

    protected override async Task before_each()
    {
        await base.before_each();
        returned_id = Guid.Empty;
        theVenue = null!;
        venuePayload = null!;

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureEventsServices()
            .ConfigureEventsDatabase(Setup.Database.GetConnectionString())
            .AddMassTransitTestHarness(x =>
            {
                x.AddEventsConsumers();
            })
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<CreateVenueEndpoint>()
            .AddScoped<GetVenueByIdEndpoint>()
            .BuildServiceProvider();

        createVenueEndpoint = serviceProvider.GetRequiredService<CreateVenueEndpoint>();
        getVenueByIdEndpoint = serviceProvider.GetRequiredService<GetVenueByIdEndpoint>();
    }

    protected override async Task after_each()
    {
        await Truncate(Setup.Database.GetConnectionString());
    }

    private void a_request_to_create_a_venue()
    {
        venuePayload = new VenuePayload(venueName, street, city, postcode, capacity);
    }

    private async Task creating_the_venue()
    {
        var response = await createVenueEndpoint.CreateVenue(venuePayload);
        returned_id = (Guid)response.Value!;
    }

    private async Task requesting_the_venue()
    {
        theVenue = (await getVenueByIdEndpoint.GetVenue(returned_id)).Value!;
    }

    private void the_venue_is_created()
    {
        theVenue.Id.ShouldBe(returned_id);
        theVenue.Name.ToString().ShouldBe(venueName);
        theVenue.Address.Street.ShouldBe(street);
        theVenue.Address.City.ShouldBe(city);
        theVenue.Address.Postcode.ShouldBe(postcode.ToUpperInvariant());
        theVenue.Capacity.ShouldBe(capacity);
    }
}