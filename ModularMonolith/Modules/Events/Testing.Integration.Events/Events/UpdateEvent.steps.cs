using Controllers.Events;
using Controllers.Events.Requests;
using Controllers.Events.Venue;
using Domain.ValueObjects;
using Infrastructure.Configuration;
using Infrastructure.Events.Core.Configuration;
using MassTransit;
using MassTransit.Testing;
using Messages.Events;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testing;
using Testing.TestData;
using Event = Domain.Events.Event;

namespace Integration.Events;

public partial class UpdateEventSpecs : TruncateDbSpecification
{
    private CreateEventEndpoint createEventEndpoint = null!;
    private GetEventByIdEndpoint getEventByIdEndpoint = null!;
    private UpdateEventEndpoint updateEventEndpoint = null!;
    private CreateVenueEndpoint createVenueEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private EventPayload eventPayload = null!;
    private UpdateEventPayload updateEventPayload = null!;
    private Event theEvent = null!;
    private ITestHarness testHarness = null!;

    private Guid venue1Id;
    private Guid returned_id;
    private const string name = "wibble";
    private const string new_name = "wobble";
    private readonly DateTimeOffset event_start_date = DateTimeOffset.UtcNow.AddDays(3);
    private readonly DateTimeOffset event_end_date = DateTimeOffset.UtcNow.AddDays(3).AddHours(2);
    private readonly DateTimeOffset new_event_start_date = DateTimeOffset.UtcNow.AddDays(1);
    private readonly DateTimeOffset new_event_end_date = DateTimeOffset.UtcNow.AddDays(1).AddHours(2);
    private readonly DateTimeOffset past_event_start_date = DateTimeOffset.UtcNow.AddDays(-1);
    private readonly Money price = 12.34m;
    private readonly Money new_price = 23.45m;

    protected override async Task before_each()
    {
        await base.before_each();
        returned_id = Guid.Empty;
        eventPayload = null!;
        updateEventPayload = null!;
        theEvent = null!;

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
            .AddScoped<GetEventByIdEndpoint>()
            .AddScoped<UpdateEventEndpoint>()
            .AddScoped<CreateVenueEndpoint>()
            .BuildServiceProvider();

        testHarness = serviceProvider.GetRequiredService<ITestHarness>();
        await testHarness.Start();
        createEventEndpoint = serviceProvider.GetRequiredService<CreateEventEndpoint>();
        getEventByIdEndpoint = serviceProvider.GetRequiredService<GetEventByIdEndpoint>();
        updateEventEndpoint = serviceProvider.GetRequiredService<UpdateEventEndpoint>();
        createVenueEndpoint = serviceProvider.GetRequiredService<CreateVenueEndpoint>();

        await SeedVenues();
    }

    protected override async Task after_each()
    {
        await Truncate(Setup.Database.GetConnectionString());
        await testHarness.Stop();
    }

    private async Task SeedVenues()
    {
        var venue1 = await createVenueEndpoint.CreateVenue(VenueTestData.FirstDirectArena);
        venue1Id = (Guid)venue1.Value!;
    }

    private void a_request_to_create_an_event()
    {
        eventPayload = new EventPayload(name, event_start_date, event_end_date, venue1Id, price);
    }

    private async Task creating_the_event()
    {
        var response = await createEventEndpoint.CreateEvent(eventPayload);
        returned_id = (Guid)response.Value!;
    }

    private async Task creating_another_event()
    {
        var response = await createEventEndpoint.CreateEvent(eventPayload);
    }

    private async Task an_event_exists()
    {
        a_request_to_create_an_event();
        await creating_the_event();
    }

    private async Task another_event_at_same_venue_exists()
    {
        eventPayload = new EventPayload(new_name, new_event_start_date, new_event_end_date, venue1Id, new_price);
        await creating_another_event();
    }

    private void a_request_to_update_the_event()
    {
        updateEventPayload = new UpdateEventPayload(new_name, new_event_start_date, new_event_end_date, new_price);
    }

    private void a_request_to_update_the_event_with_a_date_in_the_past()
    {
        updateEventPayload = new UpdateEventPayload(new_name, past_event_start_date, event_end_date, new_price);
    }

    private void a_request_to_update_the_event_with_a_venue_and_time_that_will_double_book()
    {
        updateEventPayload = new UpdateEventPayload(new_name, new_event_start_date, new_event_end_date, new_price);
    }

    private async Task updating_the_event()
    {
        await updateEventEndpoint.UpdateEvent(returned_id, updateEventPayload);
    }

    private async Task updating_the_event_that_will_fail()
    {
        await updateEventEndpoint.UpdateEvent(returned_id, updateEventPayload);
    }

    private async Task requesting_the_updated_event()
    {
        theEvent = (await getEventByIdEndpoint.GetEvent(returned_id)).Value!;
    }

    private void the_event_is_updated()
    {
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(new_name);
        (theEvent.StartDate.ToUniversalTime() - new_event_start_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        (theEvent.EndDate.ToUniversalTime() - new_event_end_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        theEvent.VenueId.ShouldBe(venue1Id);
        theEvent.Price.ShouldBe(new_price);
    }

    private void an_another_integration_event_is_published()
    {
        testHarness.Published.Select<EventUpserted>()
            .Any(e =>
                e.Context.Message.Id == returned_id &&
                e.Context.Message.EventName == new_name &&
                e.Context.Message.StartDate == new_event_start_date &&
                e.Context.Message.EndDate == new_event_end_date &&
                e.Context.Message.VenueId == venue1Id &&
                e.Context.Message.Price == new_price
            ).ShouldBeTrue("Event was not published to the bus");
    }
}