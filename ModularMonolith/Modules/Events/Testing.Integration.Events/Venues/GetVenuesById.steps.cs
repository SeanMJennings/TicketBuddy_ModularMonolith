using Controllers.Events.Requests;
using Controllers.Events.Venue;
using Infrastructure.Configuration;
using Infrastructure.Events.Core.Configuration;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testing;

namespace Integration.Venues;

public partial class GetVenuesByIdSpecs : TruncateDbSpecification
{
    private CreateVenueEndpoint createVenueEndpoint = null!;
    private GetVenueByIdEndpoint getVenueByIdEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private VenuePayload venuePayload = null!;
    private Guid returned_id;
    private ActionResult<Domain.Events.Venue.Venue>? getVenueResult;
    private const string venueName = "The Apollo";
    private const string street = "123 Main Street";
    private const string city = "Manchester";
    private const string postcode = "M1 1AA";
    private const uint capacity = 25;

    protected override async Task before_each()
    {
        await base.before_each();
        returned_id = Guid.Empty;
        getVenueResult = null;
        venuePayload = null!;

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureEventsServices()
            .ConfigureEventsDatabase(Setup.Database.GetConnectionString())
            .ConfigureSharedOutboxDatabase(Setup.Database.GetConnectionString())
            .AddMassTransitTestHarness(x =>
            {
                x.AddEventsConsumers();
                x.AddSharedOutbox();
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

    private async Task a_venue_exists()
    {
        venuePayload = new VenuePayload(venueName, street, city, postcode, capacity);
        var response = await createVenueEndpoint.CreateVenue(venuePayload);
        returned_id = (Guid)response.Value!;
    }

    private async Task requesting_the_venue()
    {
        getVenueResult = await getVenueByIdEndpoint.GetVenue(returned_id);
    }

    private async Task requesting_a_non_existent_venue()
    {
        getVenueResult = await getVenueByIdEndpoint.GetVenue(Guid.CreateVersion7());
    }

    private void the_venue_is_returned()
    {
        getVenueResult.ShouldNotBeNull();
        getVenueResult.Value.ShouldNotBeNull();
        getVenueResult.Value.Id.ShouldBe(returned_id);
        getVenueResult.Value.Name.ToString().ShouldBe(venueName);
    }

    private void a_not_found_response_is_returned()
    {
        getVenueResult.ShouldNotBeNull();
        getVenueResult.Result.ShouldBeOfType<NotFoundResult>();
    }
}