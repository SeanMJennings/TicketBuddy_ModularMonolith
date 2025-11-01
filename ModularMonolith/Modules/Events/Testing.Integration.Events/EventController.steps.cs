using System.ComponentModel.DataAnnotations;
using Application.Events.Commands;
using Application.Events.IntegrationMessageConsumers;
using Application.Events.Queries;
using BDD;
using Controllers.Events;
using Controllers.Events.Requests;
using Domain.Primitives;
using Infrastructure.Configuration;
using Infrastructure.Events.Persistence;
using Integration.Events.Messaging;
using Integration.Tickets.Messaging.Messages;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using Shouldly;
using Testcontainers.PostgreSql;
using Infrastructure.Events.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Event = Domain.Events.Entities.Event;

namespace Integration;

public partial class EventControllerSpecs : TruncateDbSpecification
{
    private EventController eventController = null!;
    private EventSoldOutConsumer eventSoldOutConsumer = null!;
    private ServiceProvider serviceProvider = null!;
    private EventPayload eventPayload = null!;
    private UpdateEventPayload updateEventPayload = null!;
    private ValidationException theError = null!;
    private Event theEvent = null!;
    private List<Event> theEvents = [];

    private Guid returned_id;
    private Guid another_id;
    private Guid third_id;
    private const string name = "wibble";
    private const string new_name = "wobble";
    private readonly DateTimeOffset event_start_date = DateTimeOffset.UtcNow.AddDays(3);
    private readonly DateTimeOffset event_end_date = DateTimeOffset.UtcNow.AddDays(3).AddHours(2);
    private readonly DateTimeOffset new_event_start_date = DateTimeOffset.UtcNow.AddDays(1);
    private readonly DateTimeOffset new_event_end_date = DateTimeOffset.UtcNow.AddDays(1).AddHours(2);
    private readonly DateTimeOffset past_event_start_date = DateTimeOffset.UtcNow.AddDays(-1);
    private const decimal price = 12.34m;
    private const decimal new_price = 23.45m;
    private static PostgreSqlContainer database = null!;
    private ITestHarness testHarness = null!;

    protected override async Task before_all()
    {
        database = new PostgreSqlBuilder()
            .WithDatabase("TicketBuddy")
            .WithUsername("sa")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1434, true)
            .WithReuse(true)
            .Build();
        await database.StartAsync();
        Migration.Upgrade(database.GetConnectionString());
    }
    
    protected override Task before_each()
    {
        base.before_each();
        returned_id = Guid.Empty;
        another_id = Guid.Empty;
        third_id = Guid.Empty;
        eventPayload = null!;
        updateEventPayload = null!;
        theError = null!;
        theEvent = null!;
        theEvents = [];
        eventSoldOutConsumer = null!;
        
        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureEventsServices()
            .ConfigureEventsDatabase(database.GetConnectionString())
            .AddMassTransitTestHarness(x =>
            {
                x.AddEventsConsumers();
            })
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<EventController>()
            .BuildServiceProvider();
        
        testHarness = serviceProvider.GetRequiredService<ITestHarness>();
        testHarness.Start().Await();
        eventController = serviceProvider.GetRequiredService<EventController>();
        eventSoldOutConsumer = serviceProvider.GetRequiredService<EventSoldOutConsumer>();
        return Task.CompletedTask;
    }

    protected override async Task after_each()
    {
        await Truncate(database.GetConnectionString());
        await testHarness.Stop();
    }

    protected override async Task after_all()
    {
        await database.StopAsync();
        await database.DisposeAsync();
    }

    private void a_request_to_create_an_event()
    {
        create_content(name, event_start_date, event_end_date, Venue.FirstDirectArenaLeeds, price);
    }

    private void a_request_to_create_an_event_imminently()
    {
        create_content(name, DateTimeOffset.UtcNow.AddSeconds(1), DateTimeOffset.UtcNow.AddSeconds(2), Venue.FirstDirectArenaLeeds, price);
    }
    
    private void a_request_to_create_an_event_with_a_date_in_the_past()
    {
        create_content(name, past_event_start_date, event_end_date, Venue.FirstDirectArenaLeeds, price);
    }

    private void a_request_to_create_an_event_with_the_same_venue_and_time()
    {
        create_content(new_name, event_start_date, event_end_date, Venue.FirstDirectArenaLeeds, new_price);
    }
    
    private void a_request_to_update_the_event_with_a_date_in_the_past()
    {
        create_update_content(new_name, past_event_start_date, event_end_date, new_price);
    }

    private void create_content(string the_name, DateTimeOffset the_event_date, DateTimeOffset the_event_end_date, Venue venue, decimal thePrice)
    {
        eventPayload = new EventPayload(the_name, the_event_date, the_event_end_date, venue, thePrice);
    }    
    
    private void create_update_content(string the_name, DateTimeOffset the_event_date, DateTimeOffset the_event_end_date, decimal thePrice)
    {
        updateEventPayload = new UpdateEventPayload(the_name, the_event_date, the_event_end_date, thePrice);
    }

    private void a_request_to_create_another_event()
    {
        create_content(new_name, event_start_date.AddDays(1), event_end_date.AddDays(1), Venue.EmiratesOldTraffordManchester, new_price);
    }

    private void a_request_to_create_third_event()
    {
        create_content("third event", event_start_date.AddDays(-1), event_end_date.AddDays(-1), Venue.PrincipalityStadiumCardiff, 34.56m);
    }
    
    private void a_request_to_update_the_event()
    {
        create_update_content(new_name, new_event_start_date, new_event_end_date, new_price);
    }

    private void a_request_to_update_the_event_with_a_venue_and_time_that_will_double_book()
    {
        create_update_content(new_name, new_event_start_date, new_event_end_date, new_price);
    }

    private async Task creating_the_event()
    {
        var response = await eventController.CreateEvent(eventPayload);
        returned_id = Guid.Parse(response.Value!.ToString()!);
    }    
    
    private async Task creating_the_event_that_will_fail()
    {
        try
        {
            await eventController.CreateEvent(eventPayload);
        }
        catch (ValidationException e)
        {
            theError = e;
        }
    }
    
    private async Task creating_another_event()
    {
        var response = await eventController.CreateEvent(eventPayload);
        another_id = Guid.Parse(response.Value!.ToString()!);
    }
    
    private async Task creating_third_event()
    {
        var response = await eventController.CreateEvent(eventPayload);
        third_id = Guid.Parse(response.Value!.ToString()!);
    }
    
    private async Task updating_the_event()
    {
        await eventController.UpdateEvent(returned_id, updateEventPayload);
    }
    
    private async Task updating_the_event_that_will_fail()
    {
        try
        {
            await eventController.UpdateEvent(returned_id, updateEventPayload);
        }
        catch (ValidationException e)
        {
            theError = e;
        }
    }
    
    private async Task an_event_exists()
    {
        a_request_to_create_an_event();
        await creating_the_event();
    }

    private void it_has_sold_out()
    {
        // would prefer to use the test harness here but haven't got it working yet
        var mockContext = Substitute.For<ConsumeContext<EventSoldOut>>();
        mockContext.Message.Returns(new EventSoldOut{ EventId = returned_id});
        eventSoldOutConsumer.Consume(mockContext).Await();
    }

    private static void a_short_wait()
    {
        Thread.Sleep(2000);
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

    private async Task another_event_at_same_venue_exists()
    {
        create_content(new_name, new_event_start_date, new_event_end_date, Venue.FirstDirectArenaLeeds, new_price);
        await creating_another_event();
    }

    private async Task requesting_the_event()
    {
        theEvent = (await eventController.GetEvent(returned_id)).Value!;
    }
    
    private async Task requesting_the_updated_event()
    {
        theEvent = (await eventController.GetEvent(returned_id)).Value!;
    }
    
    private async Task listing_the_events()
    {
        theEvents = (await eventController.GetEvents()).ToList();
    }

    private void the_event_is_created()
    {
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(name);
        (theEvent.StartDate.ToUniversalTime() - event_start_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        (theEvent.EndDate.ToUniversalTime() - event_end_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        theEvent.Venue.ShouldBe(Venue.FirstDirectArenaLeeds);
        theEvent.Price.ShouldBe(price);
    }
    
    private void the_event_is_not_created()
    {
        theError.Message.ShouldContain("Event date cannot be in the past");
    }

    private void the_user_is_informed_that_the_venue_is_unavailable()
    {
        theError.Message.ShouldContain("Venue is not available at the selected time");
    }
    
    private void the_event_is_not_updated()
    {
        theError.Message.ShouldContain("Event date cannot be in the past");
    }
    
    private void the_event_is_updated()
    {
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(new_name);
        (theEvent.StartDate.ToUniversalTime() - new_event_start_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        (theEvent.EndDate.ToUniversalTime() - new_event_end_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        theEvent.Venue.ShouldBe(Venue.FirstDirectArenaLeeds);
        theEvent.Price.ShouldBe(new_price);
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

    private void the_event_is_marked_as_sold_out()
    {
        theEvent.Id.ShouldBe(returned_id);
        theEvent.IsSoldOut.ShouldBeTrue();
    }

    private void an_integration_event_is_published()
    {
        testHarness.Published.Select<EventUpserted>()
            .Any(e => 
                e.Context.Message.Id == returned_id && 
                e.Context.Message.EventName == name &&
                e.Context.Message.StartDate == event_start_date &&
                e.Context.Message.EndDate == event_end_date &&
                e.Context.Message.Venue == Venue.FirstDirectArenaLeeds &&
                e.Context.Message.Price == price
                ).ShouldBeTrue("Event was not published to the bus");
    }

    private void an_another_integration_event_is_published()
    {
        testHarness.Published.Select<EventUpserted>()
            .Any(e => 
                e.Context.Message.Id == returned_id && 
                e.Context.Message.EventName == new_name &&
                e.Context.Message.StartDate == new_event_start_date &&
                e.Context.Message.EndDate == new_event_end_date &&
                e.Context.Message.Venue == Venue.FirstDirectArenaLeeds &&
                e.Context.Message.Price == new_price
                ).ShouldBeTrue("Event was not published to the bus");
    }
}