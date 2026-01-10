﻿using System.Net;
using System.Net.Http.Json;
using System.Text;
using Controllers.Events;
using Controllers.Events.Requests;
using Domain.Events;
using Domain.ValueObjects;
using Application;
using MassTransit.Testing;
using Messages.Events;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testing;
using Testing.Containers;
using Testing.TestData;

namespace Component.Api;

public partial class EventApiSpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private HttpContent content = null!;

    private Guid venue1Id;
    private Guid venue2Id;
    private Guid venue3Id;
    private Guid returned_id;
    private Guid another_id;
    private Guid third_id;
    private Guid returned_venue_id;
    private HttpStatusCode response_code;
    private const string application_json = "application/json";
    private const string name = "wibble";
    private const string new_name = "wobble";
    private readonly DateTimeOffset event_start_date = DateTimeOffset.UtcNow.AddDays(3);
    private readonly DateTimeOffset event_end_date = DateTimeOffset.UtcNow.AddDays(3).AddHours(2);
    private readonly DateTimeOffset new_event_start_date = DateTimeOffset.UtcNow.AddDays(1);
    private readonly DateTimeOffset new_event_end_date = DateTimeOffset.UtcNow.AddDays(1).AddHours(2);
    private readonly Money price = 12.34m;
    private readonly Money new_price = 23.45m;
    private static PostgreSqlContainer database = null!;
    private static RabbitMqContainer rabbit = null!;
    private ITestHarness testHarness = null!;

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        database.Migrate();
        rabbit = RabbitMq.CreateContainer();
        await rabbit.StartAsync();
    }
    
    protected override async Task before_each()
    {
        content = null!;
        returned_id = Guid.Empty;
        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString());
        client = factory.CreateClient();
        testHarness = factory.Services.GetRequiredService<ITestHarness>();
        client.DefaultRequestHeaders.Add(UserHeaders.UserType, nameof(UserType.Admin));
        await testHarness.Start();
        await SeedVenues();
    }

    private async Task SeedVenues()
    {
        var venue1Response = await client.PostAsJsonAsync(Routes.Venues, VenueTestData.FirstDirectArena);
        venue1Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        venue1Id = JsonSerialization.Deserialize<Guid>(await venue1Response.Content.ReadAsStringAsync());

        var venue2Response = await client.PostAsJsonAsync(Routes.Venues, VenueTestData.OldTrafford);
        venue2Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        venue2Id = JsonSerialization.Deserialize<Guid>(await venue2Response.Content.ReadAsStringAsync());

        var venue3Response = await client.PostAsJsonAsync(Routes.Venues, VenueTestData.PrincipalityStadium);
        venue3Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        venue3Id = JsonSerialization.Deserialize<Guid>(await venue3Response.Content.ReadAsStringAsync());
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
        await rabbit.StopAsync();
        await rabbit.DisposeAsync();
    }

    private void a_request_to_create_an_event()
    {
        create_content(name, event_start_date, event_end_date, venue1Id, price);
    }    
    
    private void a_request_to_create_an_event_as_a_non_admin_user()
    {
        a_request_to_create_an_event();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(UserHeaders.UserType, nameof(UserType.Customer));
    }    

    private void create_content(string theName, DateTimeOffset theEventDate, DateTimeOffset theEventEndDate, Guid venueId, decimal thePrice)
    {
        content = new StringContent(
            JsonSerialization.Serialize(new EventPayload(theName, theEventDate, theEventEndDate, venueId, thePrice)),
            Encoding.UTF8,
            application_json);
    }    
    
    private void create_update_content(string theName, DateTimeOffset theEventDate, DateTimeOffset theEventEndDate, decimal thePrice)
    {
        content = new StringContent(
            JsonSerialization.Serialize(new UpdateEventPayload(theName, theEventDate, theEventEndDate, thePrice)),
            Encoding.UTF8,
            application_json);
    }

    private void a_request_to_create_another_event()
    {
        create_content(new_name, event_start_date.AddDays(1), event_end_date.AddDays(1), venue2Id, new_price);
    }

    private void a_request_to_create_third_event()
    {
        create_content("third event", event_start_date.AddDays(-1), event_end_date.AddDays(-1), venue3Id, 34.56m);
    }
    
    private void a_request_to_update_the_event()
    {
        create_update_content(new_name, new_event_start_date, new_event_end_date, new_price);
    }
    
    private static void an_admin_user_exists() {}

    private async Task creating_the_event()
    {
        var response = await client.PostAsync(Routes.Events, content);
        response_code = response.StatusCode;
        content = response.Content;
        response_code.ShouldBe(HttpStatusCode.Created);
        returned_id = JsonSerialization.Deserialize<Guid>(await content.ReadAsStringAsync());
    }    
    
    private async Task creating_the_event_that_should_fail()
    {
        var response = await client.PostAsync(Routes.Events, content);
        response_code = response.StatusCode;
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
        client.DefaultRequestHeaders.Clear();
    }
    
    private async Task listing_the_events_as_an_anonymous_user()
    {
        client.DefaultRequestHeaders.Clear();
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
        theEvent.VenueId.ShouldBe(venue1Id);
        theEvent.Price.ShouldBe(price);
    }

    private void the_event_creation_is_forbidden()
    {
        response_code.ShouldBe(HttpStatusCode.Forbidden);
    }
    
    private async Task the_event_is_updated()
    {
        var theEvent = JsonSerialization.Deserialize<Event>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(new_name);
        (theEvent.StartDate.ToUniversalTime() - new_event_start_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        (theEvent.EndDate.ToUniversalTime() - new_event_end_date.ToUniversalTime()).TotalMilliseconds.ShouldBeLessThan(1);
        theEvent.VenueId.ShouldBe(venue1Id);
        theEvent.Price.ShouldBe(new_price);
    }   
    
    private async Task the_events_are_returned()
    {
        var theEvents = JsonSerialization.Deserialize<IReadOnlyList<Event>>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvents.Count.ShouldBe(1);
        theEvents[0].Id.ShouldBe(returned_id);
        theEvents[0].EventName.ToString().ShouldBe(name);
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
                e.Context.Message.VenueId == venue1Id &&
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
                e.Context.Message.VenueId == venue1Id &&
                e.Context.Message.Price == new_price
                ).ShouldBeTrue("Event was not published to the bus");
    }

    private void a_request_to_create_a_venue()
    {
        create_venue_content(new VenuePayload("Royal Albert Hall", "Kensington Gore", "London", "SW7 2AP", 30));
    }

    private void a_request_to_create_a_venue_as_a_non_admin_user()
    {
        a_request_to_create_a_venue();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(UserHeaders.UserType, nameof(UserType.Customer));
    }

    private void create_venue_content(VenuePayload payload)
    {
        content = new StringContent(
            JsonSerialization.Serialize(payload),
            Encoding.UTF8,
            application_json);
    }

    private async Task creating_the_venue()
    {
        var response = await client.PostAsync(Routes.Venues, content);
        response_code = response.StatusCode;
        content = response.Content;
        response_code.ShouldBe(HttpStatusCode.Created);
        returned_venue_id = JsonSerialization.Deserialize<Guid>(await content.ReadAsStringAsync());
    }

    private async Task creating_the_venue_that_should_fail()
    {
        var response = await client.PostAsync(Routes.Venues, content);
        response_code = response.StatusCode;
    }

    private void a_venue_exists()
    {
        returned_venue_id = venue1Id;
    }

    private async Task requesting_the_venue()
    {
        var response = await client.GetAsync(Routes.Venues + $"/{returned_venue_id}");
        response_code = response.StatusCode;
        content = response.Content;
    }

    private async Task requesting_the_venue_as_an_anonymous_user()
    {
        client.DefaultRequestHeaders.Clear();
        var response = await client.GetAsync(Routes.Venues + $"/{returned_venue_id}");
        response_code = response.StatusCode;
        content = response.Content;
    }

    private async Task listing_the_venues_as_an_anonymous_user()
    {
        client.DefaultRequestHeaders.Clear();
        var response = await client.GetAsync(Routes.Venues);
        response_code = response.StatusCode;
        content = response.Content;
    }

    private async Task the_venue_is_created()
    {
        var theVenue = JsonSerialization.Deserialize<Domain.Events.Venue.Venue>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theVenue.Id.ShouldBe(returned_venue_id);
        theVenue.Name.ToString().ShouldBe("Royal Albert Hall");
        theVenue.Address.Street.ShouldBe("Kensington Gore");
        theVenue.Address.City.ShouldBe("London");
        theVenue.Address.Postcode.ShouldBe("SW7 2AP");
        theVenue.Capacity.ShouldBe((uint)30);
    }

    private void the_venue_creation_is_forbidden()
    {
        response_code.ShouldBe(HttpStatusCode.Forbidden);
    }

    private async Task the_venues_are_returned()
    {
        var theVenues = JsonSerialization.Deserialize<IReadOnlyList<Domain.Events.Venue.Venue>>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theVenues.Count.ShouldBeGreaterThanOrEqualTo(3);
        theVenues.Any(v => v.Id == venue1Id).ShouldBeTrue();
    }

    private async Task the_venue_is_returned()
    {
        var theVenue = JsonSerialization.Deserialize<Domain.Events.Venue.Venue>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theVenue.Id.ShouldBe(returned_venue_id);
        theVenue.Name.ToString().ShouldBe(VenueTestData.FirstDirectArena.Name);
    }

    private void a_venue_integration_event_is_published()
    {
        testHarness.Published.Select<VenueUpserted>()
            .Any(v =>
                v.Context.Message.Id == returned_venue_id &&
                v.Context.Message.Name == "Royal Albert Hall" &&
                v.Context.Message.Capacity == 30
                ).ShouldBeTrue("VenueUpserted was not published to the bus");
    }

    // Update venue tests
    private const string updated_venue_name = "Royal Albert Hall - Renovated";
    private const string updated_street = "123 Updated Street";
    private const string updated_city = "Manchester";
    private const string updated_postcode = "M1 1AA";
    private const uint updated_capacity = 40;

    private void a_request_to_update_the_venue()
    {
        var payload = new
        {
            Name = updated_venue_name,
            Street = updated_street,
            City = updated_city,
            Postcode = updated_postcode,
            Capacity = updated_capacity
        };
        content = new StringContent(
            JsonSerialization.Serialize(payload),
            Encoding.UTF8,
            application_json);
    }

    private async Task updating_the_venue()
    {
        var response = await client.PutAsync($"{Routes.Venues}/{returned_venue_id}", content);
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.NoContent);
    }

    private async Task requesting_the_updated_venue()
    {
        var response = await client.GetAsync($"{Routes.Venues}/{returned_venue_id}");
        response_code = response.StatusCode;
        content = response.Content;
    }

    private async Task the_venue_is_updated()
    {
        var theVenue = JsonSerialization.Deserialize<Domain.Events.Venue.Venue>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        theVenue.Id.ShouldBe(returned_venue_id);
        theVenue.Name.ToString().ShouldBe(updated_venue_name);
        theVenue.Address.Street.ShouldBe(updated_street);
        theVenue.Address.City.ShouldBe(updated_city);
        theVenue.Address.Postcode.ShouldBe(updated_postcode.ToUpperInvariant());
        theVenue.Capacity.ShouldBe(updated_capacity);
    }

    private void a_request_to_update_the_venue_as_a_non_admin_user()
    {
        a_request_to_update_the_venue();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(UserHeaders.UserType, nameof(UserType.Customer));
    }

    private async Task updating_the_venue_that_should_fail()
    {
        var response = await client.PutAsync($"{Routes.Venues}/{returned_venue_id}", content);
        response_code = response.StatusCode;
    }

    private void the_venue_update_is_forbidden()
    {
        response_code.ShouldBe(HttpStatusCode.Forbidden);
    }
}