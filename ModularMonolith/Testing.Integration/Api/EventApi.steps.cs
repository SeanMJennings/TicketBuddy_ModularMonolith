using System.Net;
using System.Text;
using BDD;
using Controllers.Events;
using Controllers.Events.Requests;
using Domain.Events.Entities;
using Domain.Events.Primitives;
using Integration.Events.Messaging;
using Integration.Tickets.Messaging.Messages;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using Shouldly;
using Testcontainers.PostgreSql;
using WebHost;

namespace Integration.Api;

public partial class EventApiSpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private HttpContent content = null!;

    private Guid returned_id;
    private Guid another_id;
    private Guid third_id;
    private HttpStatusCode response_code;
    private const string application_json = "application/json";
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

    protected override void before_all()
    {
        database = new PostgreSqlBuilder()
            .WithDatabase("TicketBuddy")
            .WithUsername("sa")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1433, true)
            .Build();
        database.StartAsync().Await();
        Migration.Upgrade(database.GetConnectionString());
    }
    
    protected override void before_each()
    {
        base.before_each();
        content = null!;
        returned_id = Guid.Empty;
        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString());
        client = factory.CreateClient();
        testHarness = factory.Services.GetRequiredService<ITestHarness>();
        testHarness.Start().Await();
    }

    protected override void after_each()
    {
        Truncate(database.GetConnectionString());
        testHarness.Stop().Await();
        client.Dispose();
        factory.Dispose();
    }

    protected override void after_all()
    {
        database.StopAsync().Await();
        database.DisposeAsync().GetAwaiter().GetResult();
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
        create_content(new_name, past_event_start_date, event_end_date, Venue.EmiratesOldTraffordManchester, new_price);
    }

    private void create_content(string the_name, DateTimeOffset the_event_date, DateTimeOffset the_event_end_date, Venue venue, decimal thePrice)
    {
        content = new StringContent(
            JsonSerialization.Serialize(new EventPayload(the_name, the_event_date, the_event_end_date, venue, thePrice)),
            Encoding.UTF8,
            application_json);
    }    
    
    private void create_update_content(string the_name, DateTimeOffset the_event_date, DateTimeOffset the_event_end_date, decimal thePrice)
    {
        content = new StringContent(
            JsonSerialization.Serialize(new UpdateEventPayload(the_name, the_event_date, the_event_end_date, thePrice)),
            Encoding.UTF8,
            application_json);
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

    private void creating_the_event()
    {
        var response = client.PostAsync(Routes.Events, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
        response_code.ShouldBe(HttpStatusCode.Created);
        returned_id = JsonSerialization.Deserialize<Guid>(content.ReadAsStringAsync().Await());
    }    
    
    private void creating_the_event_that_will_fail()
    {
        var response = client.PostAsync(Routes.Events, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void creating_another_event()
    {
        var response = client.PostAsync(Routes.Events, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        another_id = JsonSerialization.Deserialize<Guid>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
    
    private void creating_third_event()
    {
        var response = client.PostAsync(Routes.Events, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        third_id = JsonSerialization.Deserialize<Guid>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
    
    private void updating_the_event()
    {
        var response = client.PutAsync(Routes.Events + $"/{returned_id}", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.NoContent);
    }
    
    private void updating_the_event_that_will_fail()
    {
        var response = client.PutAsync(Routes.Events + $"/{returned_id}", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void an_event_exists()
    {
        a_request_to_create_an_event();
        creating_the_event();
    }

    private void it_has_sold_out()
    {
        testHarness.Bus.Publish(new EventSoldOut
        {
            EventId = returned_id,
        }).Await();
        testHarness.Consumed.Any<EventSoldOut>(x => x.Context.Message.EventId == returned_id).Await();
    }

    private static void a_short_wait()
    {
        Thread.Sleep(2000);
    }
    
    private void an_imminent_event_exists()
    {
        a_request_to_create_an_event_imminently();
        creating_the_event_that_will_fail();
    }
    
    private void another_event_exists()
    {
        a_request_to_create_another_event();
        creating_another_event();
    }

    private void a_third_event_exists()
    {
        a_request_to_create_third_event();
        creating_third_event();
    }

    private void another_event_at_same_venue_exists()
    {
        create_content(new_name, new_event_start_date, new_event_end_date, Venue.FirstDirectArenaLeeds, new_price);
        creating_another_event();
    }

    private void requesting_the_event()
    {
        var response = client.GetAsync(Routes.Events + $"/{returned_id}").GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void requesting_the_updated_event()
    {
        var response = client.GetAsync(Routes.Events + $"/{returned_id}").GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void listing_the_events()
    {
        var response = client.GetAsync(Routes.Events).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }

    private void the_event_is_created()
    {

        var theEvent = JsonSerialization.Deserialize<Event>(content.ReadAsStringAsync().Await());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(name);
        (theEvent.StartDate.ToUniversalTime() - event_start_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        (theEvent.EndDate.ToUniversalTime() - event_end_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        theEvent.Venue.ShouldBe(Venue.FirstDirectArenaLeeds);
        theEvent.Price.ShouldBe(price);
    }
    
    private void the_event_is_not_created()
    {
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        var apiError = JsonSerialization.Deserialize<ApiError>(content.ReadAsStringAsync().Await());
        apiError.Errors[0].ShouldContain("Event date cannot be in the past");
    }

    private void the_user_is_informed_that_the_venue_is_unavailable()
    {
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        var apiError = JsonSerialization.Deserialize<ApiError>(content.ReadAsStringAsync().Await());
        apiError.Errors[0].ShouldContain("Venue is not available at the selected time");
    }
    
    private void the_event_is_not_updated()
    {
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        var apiError = JsonSerialization.Deserialize<ApiError>(content.ReadAsStringAsync().Await());
        apiError.Errors[0].ShouldContain("Event date cannot be in the past");
    }
    
    private void the_event_is_updated()
    {
        var theEvent = JsonSerialization.Deserialize<Event>(content.ReadAsStringAsync().Await());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(new_name);
        (theEvent.StartDate.ToUniversalTime() - new_event_start_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        (theEvent.EndDate.ToUniversalTime() - new_event_end_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        theEvent.Venue.ShouldBe(Venue.FirstDirectArenaLeeds);
        theEvent.Price.ShouldBe(new_price);
    }    
    
    private void the_events_are_listed_earliest_first()
    {
        var theEvents = JsonSerialization.Deserialize<IReadOnlyList<Event>>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvents.Count.ShouldBe(3);
        theEvents.Single(e => e.Id == returned_id).EventName.ToString().ShouldBe(name);
        theEvents.Single(e => e.Id == another_id).EventName.ToString().ShouldBe(new_name);
        theEvents[0].Id.ShouldBe(third_id);
        theEvents[1].Id.ShouldBe(returned_id);
        theEvents[2].Id.ShouldBe(another_id);
    }

    private void the_events_are_listed_without_the_past_event()
    {
        var theEvent = JsonSerialization.Deserialize<IReadOnlyList<Event>>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvent.Count.ShouldBe(1);
        theEvent.Single().Id.ShouldBe(returned_id);
        theEvent.Single().EventName.ToString().ShouldBe(name);
    }

    private void the_event_is_marked_as_sold_out()
    {
        var theEvent = JsonSerialization.Deserialize<Event>(content.ReadAsStringAsync().Await());
        response_code.ShouldBe(HttpStatusCode.OK);
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