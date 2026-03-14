using Controllers.Events;
using Controllers.Events.Requests;
using Controllers.Events.Venue;
using Domain.ValueObjects;
using Infrastructure.Configuration;
using Infrastructure.Events.Core.Configuration;
using MassTransit;
using Messages.Tickets;
using Messaging.Events;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Testing;
using Testing.TestData;
using Event = Domain.Events.Event;

namespace Integration.Events;

public partial class EventSoldOutConsumerSpecs : TruncateDbSpecification
{
    private CreateEventEndpoint createEventEndpoint = null!;
    private GetEventByIdEndpoint getEventByIdEndpoint = null!;
    private CreateVenueEndpoint createVenueEndpoint = null!;
    private EventSoldOutConsumer eventSoldOutConsumer = null!;
    private ServiceProvider serviceProvider = null!;
    private EventPayload eventPayload = null!;
    private Event theEvent = null!;

    private Guid venue1Id;
    private Guid returned_id;
    private const string name = "wibble";
    private readonly DateTimeOffset event_start_date = DateTimeOffset.UtcNow.AddDays(3);
    private readonly DateTimeOffset event_end_date = DateTimeOffset.UtcNow.AddDays(3).AddHours(2);
    private readonly Money price = 12.34m;

    protected override async Task before_each()
    {
        await base.before_each();
        returned_id = Guid.Empty;
        eventPayload = null!;
        theEvent = null!;
        eventSoldOutConsumer = null!;

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
            .AddScoped<CreateVenueEndpoint>()
            .BuildServiceProvider();

        createEventEndpoint = serviceProvider.GetRequiredService<CreateEventEndpoint>();
        getEventByIdEndpoint = serviceProvider.GetRequiredService<GetEventByIdEndpoint>();
        createVenueEndpoint = serviceProvider.GetRequiredService<CreateVenueEndpoint>();
        eventSoldOutConsumer = serviceProvider.GetRequiredService<EventSoldOutConsumer>();

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

    private async Task an_event_exists()
    {
        a_request_to_create_an_event();
        await creating_the_event();
    }

    private void it_has_sold_out()
    {
        var mockContext = Substitute.For<ConsumeContext<EventSoldOut>>();
        mockContext.Message.Returns(new EventSoldOut { EventId = returned_id });
        eventSoldOutConsumer.Consume(mockContext).GetAwaiter().GetResult();
    }

    private async Task marking_non_existent_event_as_sold_out()
    {
        var mockContext = Substitute.For<ConsumeContext<EventSoldOut>>();
        mockContext.Message.Returns(new EventSoldOut { EventId = Guid.CreateVersion7() });
        await eventSoldOutConsumer.Consume(mockContext);
    }

    private async Task requesting_the_event()
    {
        theEvent = (await getEventByIdEndpoint.GetEvent(returned_id)).Value!;
    }

    private void the_event_is_marked_as_sold_out()
    {
        theEvent.Id.ShouldBe(returned_id);
        theEvent.IsSoldOut.ShouldBeTrue();
    }
}