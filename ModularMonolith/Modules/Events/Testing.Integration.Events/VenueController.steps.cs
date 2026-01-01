using Controllers.Events.Requests;
using Controllers.Events.Venue;
using Domain.Events.Venue;
using Infrastructure.Configuration;
using Infrastructure.Events.Core.Configuration;
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
    private ServiceProvider serviceProvider = null!;
    private VenuePayload venuePayload = null!;
    private Guid returned_id;
    private Venue theVenue = null!;
    private const string venueName = "The Apollo";
    private const string street = "123 Main Street";
    private const string city = "Manchester";
    private const string postcode = "M1 1AA";
    private const uint capacity = 25;
    private static PostgreSqlContainer database = null!;

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        database.Migrate();
    }

    protected override Task before_each()
    {
        base.before_each();
        returned_id = Guid.Empty;
        theVenue = null!;
        venuePayload = null!;

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureEventsServices()
            .ConfigureEventsDatabase(database.GetConnectionString())
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<CreateVenueEndpoint>()
            .AddScoped<GetVenueByIdEndpoint>()
            .BuildServiceProvider();

        createVenueEndpoint = serviceProvider.GetRequiredService<CreateVenueEndpoint>();
        getVenueByIdEndpoint = serviceProvider.GetRequiredService<GetVenueByIdEndpoint>();

        return Task.CompletedTask;
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
}