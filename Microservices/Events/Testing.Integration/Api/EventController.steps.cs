﻿using System.Net;
using System.Text;
using Api.Hosting;
using Api.Requests;
using BDD;
using Domain.Entities;
using Events.Integration.Messaging.Outbound.Messages;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using Shared.Testing;
using Shouldly;
using Testcontainers.MsSql;

namespace Integration.Api;

public partial class EventControllerSpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private HttpContent content = null!;

    private Guid returned_id;
    private Guid another_id;
    private HttpStatusCode response_code;
    private const string application_json = "application/json";
    private const string name = "wibble";
    private const string new_name = "wobble";
    private static MsSqlContainer database = null!;
    private ITestHarness testHarness = null!;

    protected override void before_all()
    {
        database = new MsSqlBuilder().WithPortBinding(1433, true).Build();
        database.StartAsync().Await();
        database.ExecScriptAsync("CREATE DATABASE [TicketBuddy.Events]").GetAwaiter().GetResult();
        Migration.Upgrade(database.GetTicketBuddyConnectionString());
    }
    
    protected override void before_each()
    {
        base.before_each();
        content = null!;
        returned_id = Guid.Empty;
        factory = new IntegrationWebApplicationFactory<Program>(database.GetTicketBuddyConnectionString());
        client = factory.CreateClient();
        testHarness = factory.Services.GetRequiredService<ITestHarness>();
        testHarness.Start().Await();
    }

    protected override void after_each()
    {
        Truncate(database.GetTicketBuddyConnectionString());
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
        create_content(name);
    }

    private void create_content(string the_name)
    {
        content = new StringContent(
            JsonSerialization.Serialize(new EventPayload(the_name)),
            Encoding.UTF8,
            application_json);
    }

    private void a_request_to_create_another_event()
    {
        create_content(new_name);

    }

    private void a_request_to_update_the_event()
    {
        create_content(new_name);
    }

    private void creating_the_event()
    {
        var response = client.PostAsync(EventsRoutes.Events, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.Created);
        returned_id = JsonSerialization.Deserialize<Guid>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
    
    private void creating_another_event()
    {
        var response = client.PostAsync(EventsRoutes.Events, content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        another_id = JsonSerialization.Deserialize<Guid>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }      
    
    private void updating_the_event()
    {
        var response = client.PutAsync(EventsRoutes.Events + $"/{returned_id}", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.NoContent);
    }
    
    private void an_event_exists()
    {
        a_request_to_create_an_event();
        creating_the_event();
    }    
    
    private void another_event_exists()
    {
        a_request_to_create_another_event();
        creating_another_event();
    }

    private void requesting_the_event()
    {
        var response = client.GetAsync(EventsRoutes.Events + $"/{returned_id}").GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void requesting_the_updated_event()
    {
        var response = client.GetAsync(EventsRoutes.Events + $"/{returned_id}").GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void listing_the_events()
    {
        var response = client.GetAsync(EventsRoutes.Events).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }

    private void the_event_is_created()
    {
        var theEvent = JsonSerialization.Deserialize<Event>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(name);
        testHarness.Published.Select<Events.Domain.Messaging.Messages.EventUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message.Name == name).ShouldBeTrue("Event was not published to the bus");
        testHarness.Published.Select<EventUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message.Name == name).ShouldBeTrue("Event was not published to the bus");
    }
    
    private void the_event_is_updated()
    {
        var theEvent = JsonSerialization.Deserialize<Event>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvent.Id.ShouldBe(returned_id);
        theEvent.EventName.ToString().ShouldBe(new_name);
        testHarness.Published.Select<Events.Domain.Messaging.Messages.EventUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message.Name == name).ShouldBeTrue("Event was not published to the bus");
        testHarness.Published.Select<EventUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message.Name == new_name).ShouldBeTrue("Event was not published to the bus");
        testHarness.Published.Select<Events.Domain.Messaging.Messages.EventUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message.Name == name).ShouldBeTrue("Event was not published to the bus");
        testHarness.Published.Select<EventUpserted>()
            .Any(e => e.Context.Message.Id == returned_id && e.Context.Message.Name == new_name).ShouldBeTrue("Event was not published to the bus");
    }    
    
    private void the_events_are_listed()
    {
        var theEvent = JsonSerialization.Deserialize<IReadOnlyList<Event>>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        response_code.ShouldBe(HttpStatusCode.OK);
        theEvent.Count.ShouldBe(2);
        theEvent.Single(e => e.Id == returned_id).EventName.ToString().ShouldBe(name);
        theEvent.Single(e => e.Id == another_id).EventName.ToString().ShouldBe(new_name);
    }
}