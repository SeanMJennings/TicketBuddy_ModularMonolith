using Controllers.Events.Requests;
using Controllers.Events.Venue;
using Infrastructure.Configuration;
using Infrastructure.Events.Core.Configuration;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testing;

namespace Integration.Venues;

public partial class GetVenuesSpecs : TruncateDbSpecification
{
    private CreateVenueEndpoint createVenueEndpoint = null!;
    private GetVenuesEndpoint getVenuesEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private VenuePayload venuePayload = null!;
    private Guid returned_id;
    private Guid another_id;
    private Guid third_id;
    private List<Domain.Events.Venue.Venue> theVenues = [];
    private const string venueName = "The Apollo";
    private const string street = "123 Main Street";
    private const string city = "Manchester";
    private const string postcode = "M1 1AA";
    private const uint capacity = 25;
    private const string secondVenueName = "The Ritz";
    private const string secondStreet = "456 Second Street";
    private const string secondCity = "London";
    private const string secondPostcode = "SW1A 1AA";
    private const uint secondCapacity = 30;
    private const string thirdVenueName = "The Palladium";
    private const string thirdStreet = "789 Third Avenue";
    private const string thirdCity = "Birmingham";
    private const string thirdPostcode = "B1 1AA";
    private const uint thirdCapacity = 40;

    protected override async Task before_each()
    {
        await base.before_each();
        returned_id = Guid.Empty;
        another_id = Guid.Empty;
        third_id = Guid.Empty;
        theVenues = [];
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
            .AddScoped<GetVenuesEndpoint>()
            .BuildServiceProvider();

        createVenueEndpoint = serviceProvider.GetRequiredService<CreateVenueEndpoint>();
        getVenuesEndpoint = serviceProvider.GetRequiredService<GetVenuesEndpoint>();
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

    private async Task a_venue_exists()
    {
        a_request_to_create_a_venue();
        await creating_the_venue();
    }

    private void a_request_to_create_another_venue()
    {
        venuePayload = new VenuePayload(secondVenueName, secondStreet, secondCity, secondPostcode, secondCapacity);
    }

    private async Task creating_another_venue()
    {
        var response = await createVenueEndpoint.CreateVenue(venuePayload);
        another_id = (Guid)response.Value!;
    }

    private async Task another_venue_exists()
    {
        a_request_to_create_another_venue();
        await creating_another_venue();
    }

    private void a_request_to_create_third_venue()
    {
        venuePayload = new VenuePayload(thirdVenueName, thirdStreet, thirdCity, thirdPostcode, thirdCapacity);
    }

    private async Task creating_third_venue()
    {
        var response = await createVenueEndpoint.CreateVenue(venuePayload);
        third_id = (Guid)response.Value!;
    }

    private async Task a_third_venue_exists()
    {
        a_request_to_create_third_venue();
        await creating_third_venue();
    }

    private async Task listing_the_venues()
    {
        theVenues = (await getVenuesEndpoint.GetVenues()).ToList();
    }

    private void the_venues_are_listed()
    {
        theVenues.Count.ShouldBe(3);
        theVenues.Single(v => v.Id == returned_id).Name.ToString().ShouldBe(venueName);
        theVenues.Single(v => v.Id == another_id).Name.ToString().ShouldBe(secondVenueName);
        theVenues.Single(v => v.Id == third_id).Name.ToString().ShouldBe(thirdVenueName);
    }
}