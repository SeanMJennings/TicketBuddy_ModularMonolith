using System.ComponentModel.DataAnnotations;
using Controllers.Events;
using Controllers.Events.Requests;
using Controllers.Events.Venue;
using Domain.ValueObjects;
using Infrastructure.Configuration;
using Infrastructure.Events.Core.Configuration;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testing;
using Testing.TestData;
using Event = Domain.Events.Event;

namespace Integration.Events;

public partial class GetEventsSpecs : TruncateDbSpecification
{
    private CreateEventEndpoint createEventEndpoint = null!;
    private GetEventsEndpoint getEventsEndpoint = null!;
    private CreateVenueEndpoint createVenueEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private EventPayload eventPayload = null!;
    private List<Event> theEvents = [];

    private Guid venue1Id;
    private Guid venue2Id;
    private Guid venue3Id;
    private Guid returned_id;
    private Guid another_id;
    private Guid third_id;
    private const string name = "wibble";
    private const string new_name = "wobble";
    private readonly DateTimeOffset event_start_date = DateTimeOffset.UtcNow.AddDays(3);
    private readonly DateTimeOffset event_end_date = DateTimeOffset.UtcNow.AddDays(3).AddHours(2);
    private readonly Money price = 12.34m;
    private readonly Money new_price = 23.45m;

    protected override async Task before_each()
    {
        await base.before_each();
        returned_id = Guid.Empty;
        another_id = Guid.Empty;
        third_id = Guid.Empty;
        eventPayload = null!;
        theEvents = [];

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureEventsServices()
            .ConfigureEventsDatabase(Setup.Database.GetConnectionString())
            .AddMassTransitTestHarness(x =>
            {
                x.AddEventsConsumers();
            })
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<CreateEventEndpoint>()
            .AddScoped<GetEventsEndpoint>()
            .AddScoped<CreateVenueEndpoint>()
            .BuildServiceProvider();

        createEventEndpoint = serviceProvider.GetRequiredService<CreateEventEndpoint>();
        getEventsEndpoint = serviceProvider.GetRequiredService<GetEventsEndpoint>();
        createVenueEndpoint = serviceProvider.GetRequiredService<CreateVenueEndpoint>();

        await SeedVenues();
    }

    protected override async Task after_each()
    {
        await Truncate(Setup.Database.GetConnectionString());
    }

    private async Task SeedVenues()
    {
        var venue1 = await createVenueEndpoint.CreateVenue(VenueTestData.FirstDirectArena);
        venue1Id = (Guid)venue1.Value!;
        var venue2 = await createVenueEndpoint.CreateVenue(VenueTestData.OldTrafford);
        venue2Id = (Guid)venue2.Value!;
        var venue3 = await createVenueEndpoint.CreateVenue(VenueTestData.PrincipalityStadium);
        venue3Id = (Guid)venue3.Value!;
    }

    private void a_request_to_create_an_event()
    {
        eventPayload = new EventPayload(name, event_start_date, event_end_date, venue1Id, price);
    }

    private void a_request_to_create_an_event_imminently()
    {
        eventPayload = new EventPayload(name, DateTimeOffset.UtcNow.AddSeconds(1), DateTimeOffset.UtcNow.AddSeconds(2), venue1Id, price);
    }

    private void a_request_to_create_another_event()
    {
        eventPayload = new EventPayload(new_name, event_start_date.AddDays(1), event_end_date.AddDays(1), venue2Id, new_price);
    }

    private void a_request_to_create_third_event()
    {
        eventPayload = new EventPayload("third event", event_start_date.AddDays(-1), event_end_date.AddDays(-1), venue3Id, 34.56m);
    }

    private async Task creating_the_event()
    {
        var response = await createEventEndpoint.CreateEvent(eventPayload);
        returned_id = (Guid)response.Value!;
    }

    private async Task creating_the_event_that_will_fail()
    {
        try
        {
            await createEventEndpoint.CreateEvent(eventPayload);
        }
        catch (ValidationException)
        {
            // Expected for imminent events
        }
    }

    private async Task creating_another_event()
    {
        var response = await createEventEndpoint.CreateEvent(eventPayload);
        another_id = (Guid)response.Value!;
    }

    private async Task creating_third_event()
    {
        var response = await createEventEndpoint.CreateEvent(eventPayload);
        third_id = (Guid)response.Value!;
    }

    private async Task an_event_exists()
    {
        a_request_to_create_an_event();
        await creating_the_event();
    }

    private async Task an_imminent_event_exists()
    {
        a_request_to_create_an_event_imminently();
        await creating_the_event_that_will_fail();
    }

    private async Task another_event_exists()
    {
        a_request_to_create_another_event();
        await creating_another_event();
    }

    private async Task a_third_event_exists()
    {
        a_request_to_create_third_event();
        await creating_third_event();
    }

    private static void a_short_wait()
    {
        Thread.Sleep(2000);
    }

    private async Task listing_the_events()
    {
        theEvents = (await getEventsEndpoint.GetEvents()).ToList();
    }

    private void the_events_are_listed_earliest_first()
    {
        theEvents.Count.ShouldBe(3);
        theEvents.Single(e => e.Id == returned_id).EventName.ToString().ShouldBe(name);
        theEvents.Single(e => e.Id == another_id).EventName.ToString().ShouldBe(new_name);
        theEvents[0].Id.ShouldBe(third_id);
        theEvents[1].Id.ShouldBe(returned_id);
        theEvents[2].Id.ShouldBe(another_id);
    }

    private void the_events_are_listed_without_the_past_event()
    {
        theEvents.Count.ShouldBe(1);
        theEvents.Single().Id.ShouldBe(returned_id);
        theEvents.Single().EventName.ToString().ShouldBe(name);
    }
}