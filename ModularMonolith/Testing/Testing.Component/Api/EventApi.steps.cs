using System.Net;
using System.Text;
using Controllers.Events;
using Controllers.Events.Requests;
using Domain.Events.Entities;
using Domain.Primitives;
using Integration.Events.Messaging;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using Shouldly;
using Testcontainers.PostgreSql;
using Testing;
using Testing.Containers;

namespace Component.Api;

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
    private const decimal price = 12.34m;
    private const decimal new_price = 23.45m;
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
        content = null!;
        returned_id = Guid.Empty;
        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString());
        client = factory.CreateClient();
        testHarness = factory.Services.GetRequiredService<ITestHarness>();
        await testHarness.Start();
    }

    protected override async Task after_each()
    {
        await Truncate(database.GetConnectionString());
        await testHarness.Stop();
        client.Dispose();
        await factory.DisposeAsync();
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

    private async Task creating_the_event()
    {
        var response = await client.PostAsync(Routes.Events, content);
        response_code = response.StatusCode;
        content = response.Content;
        response_code.ShouldBe(HttpStatusCode.Created);
        returned_id = JsonSerialization.Deserialize<Guid>(await content.ReadAsStringAsync());
    }
    
    private async Task creating_another_event()
    {
        var response = await client.PostAsync(Routes.Events, content);
        response_code = response.StatusCode;
        another_id = JsonSerialization.Deserialize<Guid>(await response.Content.ReadAsStringAsync());
    }
    
    private async Task creating_third_event()
    {
        var response = await client.PostAsync(Routes.Events, content);
        response_code = response.StatusCode;
        third_id = JsonSerialization.Deserialize<Guid>(await response.Content.ReadAsStringAsync());
    }
    
    private async Task updating_the_event()
    {
        var response = await client.PutAsync(Routes.Events + $"/{returned_id}", content);
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.NoContent);
    }
    
    private async Task an_event_exists()
    {
        a_request_to_create_an_event();
        await creating_the_event();
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

    private async Task requesting_the_event()
    {
        var response = await client.GetAsync(Routes.Events + $"/{returned_id}");
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private async Task requesting_the_updated_event()
    {
        var response = await client.GetAsync(Routes.Events + $"/{returned_id}");
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private async Task listing_the_events()
    {
        var response = await client.GetAsync(Routes.Events);
        response_code = response.StatusCode;
        content = response.Content;
    }

    private async Task the_event_is_created()
    {
        var theEvent = JsonSerialization.Deserialize<Event>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(name);
        (theEvent.StartDate.ToUniversalTime() - event_start_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        (theEvent.EndDate.ToUniversalTime() - event_end_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        theEvent.Venue.ShouldBe(Venue.FirstDirectArenaLeeds);
        theEvent.Price.ShouldBe(price);
    }
    
    private async Task the_event_is_updated()
    {
        var theEvent = JsonSerialization.Deserialize<Event>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(new_name);
        (theEvent.StartDate.ToUniversalTime() - new_event_start_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        (theEvent.EndDate.ToUniversalTime() - new_event_end_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        theEvent.Venue.ShouldBe(Venue.FirstDirectArenaLeeds);
        theEvent.Price.ShouldBe(new_price);
    }    
    
    private async Task the_events_are_listed_earliest_first()
    {
        var theEvents = JsonSerialization.Deserialize<IReadOnlyList<Event>>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvents.Count.ShouldBe(3);
        theEvents.Single(e => e.Id == returned_id).EventName.ToString().ShouldBe(name);
        theEvents.Single(e => e.Id == another_id).EventName.ToString().ShouldBe(new_name);
        theEvents[0].Id.ShouldBe(third_id);
        theEvents[1].Id.ShouldBe(returned_id);
        theEvents[2].Id.ShouldBe(another_id);
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