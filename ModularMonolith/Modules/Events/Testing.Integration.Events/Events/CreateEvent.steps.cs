using Controllers.Events;
using Controllers.Events.Requests;
using Controllers.Events.Venue;
using Domain.ValueObjects;
using Infrastructure.Configuration;
using Infrastructure.Events.Core.Configuration;
using Infrastructure.Messaging;
using MassTransit;
using MassTransit.Testing;
using Messages.Events;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Shouldly;
using Testing;
using Testing.TestData;
using Event = Domain.Events.Event;

namespace Integration.Events;

public partial class CreateEventSpecs : TruncateDbSpecification
{
    private CreateEventEndpoint createEventEndpoint = null!;
    private GetEventByIdEndpoint getEventByIdEndpoint = null!;
    private CreateVenueEndpoint createVenueEndpoint = null!;
    private ServiceProvider serviceProvider = null!;
    private EventPayload eventPayload = null!;
    private Event theEvent = null!;
    private ITestHarness testHarness = null!;

    private Guid venue1Id;
    private Guid returned_id;
    private const string name = "wibble";
    private const string new_name = "wobble";
    private readonly DateTimeOffset event_start_date = DateTimeOffset.UtcNow.AddDays(3);
    private readonly DateTimeOffset event_end_date = DateTimeOffset.UtcNow.AddDays(3).AddHours(2);
    private readonly DateTimeOffset past_event_start_date = DateTimeOffset.UtcNow.AddDays(-1);
    private readonly Money price = 12.34m;
    private readonly Money new_price = 23.45m;

    protected override async Task before_each()
    {
        await base.before_each();
        returned_id = Guid.Empty;
        eventPayload = null!;
        theEvent = null!;

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
            .AddScoped<CreateEventEndpoint>()
            .AddScoped<GetEventByIdEndpoint>()
            .AddScoped<CreateVenueEndpoint>()
            .BuildServiceProvider();

        testHarness = serviceProvider.GetRequiredService<ITestHarness>();
        await testHarness.Start();
        createEventEndpoint = serviceProvider.GetRequiredService<CreateEventEndpoint>();
        getEventByIdEndpoint = serviceProvider.GetRequiredService<GetEventByIdEndpoint>();
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

    private void a_request_to_create_an_event_with_a_date_in_the_past()
    {
        eventPayload = new EventPayload(name, past_event_start_date, event_end_date, venue1Id, price);
    }

    private void a_request_to_create_an_event_with_the_same_venue_and_time()
    {
        eventPayload = new EventPayload(new_name, event_start_date, event_end_date, venue1Id, new_price);
    }

    private async Task creating_the_event()
    {
        var response = await createEventEndpoint.CreateEvent(eventPayload);
        returned_id = (Guid)response.Value!;
    }

    private async Task creating_the_event_that_will_fail()
    {
        await createEventEndpoint.CreateEvent(eventPayload);
    }

    private async Task an_event_exists()
    {
        a_request_to_create_an_event();
        await creating_the_event();
    }

    private async Task requesting_the_event()
    {
        theEvent = (await getEventByIdEndpoint.GetEvent(returned_id)).Value!;
    }

    private void the_event_is_created()
    {
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(name);
        (theEvent.StartDate.ToUniversalTime() - event_start_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        (theEvent.EndDate.ToUniversalTime() - event_end_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        theEvent.VenueId.ShouldBe(venue1Id);
        theEvent.Price.ShouldBe(price);
    }

    private static async Task outbox_message_is_persisted_to_the_event_schema()
    {
        await using var connection = new NpgsqlConnection(Setup.Database.GetConnectionString());
        await connection.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            """SELECT COUNT(*) FROM "Messaging"."OutboxMessage" WHERE "MessageType" LIKE '%EventUpserted%'""",
            connection);
        var count = (long)(await cmd.ExecuteScalarAsync())!;
        count.ShouldBeGreaterThan(0);
    }
}