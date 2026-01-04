using BDD;
using Controllers.Events.Requests;
using Controllers.Events.Venue;
using Domain.Events.Venue;
using Infrastructure.Configuration;
using Infrastructure.Events.Core.Configuration;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testcontainers.PostgreSql;
using Testing;
using Testing.Containers;

namespace Integration;

public partial class VenueControllerSpecs : TruncateDbSpecification
{
    private CreateVenueEndpoint createVenueEndpoint = null!;
    private GetVenueByIdEndpoint getVenueByIdEndpoint = null!;
    private GetVenuesEndpoint getVenuesEndpoint = null!;
    private UpdateVenueEndpoint updateVenueEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private VenuePayload venuePayload = null!;
    private Guid returned_id;
    private Guid another_id;
    private Guid third_id;
    private Venue theVenue = null!;
    private List<Venue> theVenues = [];
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
    private static PostgreSqlContainer database = null!;
    private ITestHarness testHarness = null!;

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        database.Migrate();
    }

    protected override async Task before_each()
    {
        await base.before_each();
        returned_id = Guid.Empty;
        another_id = Guid.Empty;
        third_id = Guid.Empty;
        theVenue = null!;
        theVenues = [];
        venuePayload = null!;

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureEventsServices()
            .ConfigureEventsDatabase(database.GetConnectionString())
            .AddMassTransitTestHarness(x =>
            {
                x.AddEventsConsumers();
            })
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<CreateVenueEndpoint>()
            .AddScoped<GetVenueByIdEndpoint>()
            .AddScoped<GetVenuesEndpoint>()
            .AddScoped<UpdateVenueEndpoint>()
            .BuildServiceProvider();

        testHarness = serviceProvider.GetRequiredService<ITestHarness>();
        await testHarness.Start();
        createVenueEndpoint = serviceProvider.GetRequiredService<CreateVenueEndpoint>();
        getVenueByIdEndpoint = serviceProvider.GetRequiredService<GetVenueByIdEndpoint>();
        getVenuesEndpoint = serviceProvider.GetRequiredService<GetVenuesEndpoint>();
        updateVenueEndpoint = serviceProvider.GetRequiredService<UpdateVenueEndpoint>();
    }

    protected override async Task after_each()
    {
        await Truncate(database.GetConnectionString());
    }

    protected override async Task after_all()
    {
        await database.StopAsync();
        await database.DisposeAsync();
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

    // Update tests
    private UpdateVenuePayload updateVenuePayload = null!;
    private const string updatedVenueName = "The Apollo Updated";
    private const string updatedStreet = "999 Updated Street";
    private const string updatedCity = "Leeds";
    private const string updatedPostcode = "LS1 1AA";
    private const uint updatedCapacity = 35;

    private void a_request_to_update_the_venue()
    {
        updateVenuePayload = new UpdateVenuePayload(
            new VenueName(updatedVenueName),
            updatedStreet,
            updatedCity,
            updatedPostcode,
            updatedCapacity);
    }

    private async Task updating_the_venue()
    {
        await updateVenueEndpoint.UpdateVenue(returned_id, updateVenuePayload);
    }

    private async Task requesting_the_updated_venue()
    {
        theVenue = (await getVenueByIdEndpoint.GetVenue(returned_id)).Value!;
    }

    private void the_venue_is_updated()
    {
        theVenue.Id.ShouldBe(returned_id);
        theVenue.Name.ToString().ShouldBe(updatedVenueName);
        theVenue.Address.Street.ShouldBe(updatedStreet);
        theVenue.Address.City.ShouldBe(updatedCity);
        theVenue.Address.Postcode.ShouldBe(updatedPostcode.ToUpperInvariant());
        theVenue.Capacity.ShouldBe(updatedCapacity);
    }

    private void a_request_to_update_venue_to_duplicate_address()
    {
        updateVenuePayload = new UpdateVenuePayload(
            new VenueName(updatedVenueName),
            secondStreet,
            secondCity,
            secondPostcode,
            updatedCapacity);
    }

    private void a_venue_upserted_message_is_published()
    {
        testHarness.Published.Select<Messages.Events.VenueUpserted>()
            .Any(e =>
                e.Context.Message.Id == returned_id &&
                e.Context.Message.Name == new VenueName(updatedVenueName) &&
                e.Context.Message.Capacity == updatedCapacity)
            .ShouldBeTrue();
    }
}