using System.Net;
using System.Net.Http.Json;
using System.Text;
using Controllers.Events;
using Controllers.Events.Requests;
using Controllers.Tickets.Requests;
using Domain.Primitives;
using Domain.Tickets.Queries;
using Integration.Keycloak.Users.Messaging;
using Keycloak.Client;
using Keycloak.Domain;
using RabbitMQ.Client;
using Shouldly;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Testing;
using Testing.Containers;

namespace Acceptance;

public partial class TicketBuddySpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private HttpContent content = null!;

    private Guid event_id;
    private readonly Guid user_id = Guid.NewGuid();
    private HttpStatusCode response_code;
    private const string application_json = "application/json";
    private const string first_name = "wibble";
    private const string last_name = "wobble";
    private const string email = "wibble@wobble.com";
    private readonly DateTimeOffset event_start_date = DateTimeOffset.UtcNow.AddDays(3);
    private readonly DateTimeOffset event_end_date = DateTimeOffset.UtcNow.AddDays(3).AddHours(2);
    private const decimal price = 12.34m;
    private static PostgreSqlContainer database = null!;
    private static RabbitMqContainer rabbit = null!;
    private static RedisContainer redis = null!;
    private static KeycloakContainer keycloak = null!;
    private Guid[] ticket_ids = [];
    private static string EventTickets(Guid id) => $"{Routes.Events}/{id}/tickets";

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer(1435);
        await database.StartAsync();
        database.Migrate();
        rabbit = RabbitMq.CreateContainer(5674);
        await rabbit.StartAsync();
        redis = Redis.CreateContainer(6381);
        await redis.StartAsync();
        keycloak = Testing.Containers.Keycloak.CreateContainer(new Uri($"https://{rabbit.Hostname}:{rabbit.GetMappedPublicPort(5672)}/"));
        await keycloak.StartAsync();
    }
    
    protected override Task before_each()
    {
        content = null!;
        ticket_ids = [];
        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString(), redis.GetConnectionString(), rabbit.GetConnectionString());
        client = factory.CreateClient();
        return Task.CompletedTask;
    }

    protected override async Task after_each()
    {
        await Truncate(database.GetConnectionString());
        client.Dispose();
        await factory.DisposeAsync();
    }

    protected override async Task after_all()
    {
        await database.StopAsync();
        await database.DisposeAsync();
        await rabbit.StopAsync();
        await rabbit.DisposeAsync();
        await redis.StopAsync();
        await redis.DisposeAsync();
        await keycloak.StopAsync();
        await keycloak.DisposeAsync();
    }
    
    private async Task an_event_exists()
    {
        var theContent = new StringContent(
            JsonSerialization.Serialize(new EventPayload(first_name, event_start_date, event_end_date, Venue.FirstDirectArenaLeeds, price)),
            Encoding.UTF8,
            application_json);
        var response = await client.PostAsync(Routes.Events, theContent);
        response_code = response.StatusCode;
        content = response.Content;
        response_code.ShouldBe(HttpStatusCode.Created);
        event_id = JsonSerialization.Deserialize<Guid>(await content.ReadAsStringAsync());
    }
    
    private async Task a_user_exists()
    {
        var keycloakApiHttpClient = await KeycloakAdminClient.CreateKeycloakAdminClient(
            new Uri(keycloak.GetBaseAddress()[..^1]),
            "admin-cli",
            Testing.Containers.Keycloak.AdminUserName,
            Testing.Containers.Keycloak.AdminPassword);
        
        var payload = new UserRepresentation
        {
            id = user_id,
            firstName = first_name,
            lastName = last_name,
            email = email,
            credentials =
            [
                new CredentialRepresentation
                {
                    value = first_name + last_name,
                }
            ]
        };

        var response = await keycloakApiHttpClient.PostAsJsonAsync("/admin/realms/ticketbuddy/users", payload);
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.Created);
        
        await RecreateKeycloakUserRegisteredEventFromKeycloak();
    }

    private async Task RecreateKeycloakUserRegisteredEventFromKeycloak()
    {
        var rabbitMqFactory = new ConnectionFactory
        {
            Uri = new Uri(rabbit.GetConnectionString())
        };
        var rabbitMqConnection = await rabbitMqFactory.CreateConnectionAsync();
        await using var channel = await rabbitMqConnection.CreateChannelAsync();
        var details = new Dictionary<string, string>
        {
            { "first_name", first_name },
            { "last_name", last_name },
            { "email", email }
        };
        var message = JsonSerialization.Serialize(new UserRegistered(user_id, details));
        var body = Encoding.UTF8.GetBytes(message);
        var properties = new BasicProperties
        {
            Persistent = true
        };
        await channel.BasicPublishAsync("amq.topic", "KK.EVENT.CLIENT.ticketbuddy.SUCCESS.ticketbuddy-ui.REGISTER", true, properties, body);
        await rabbitMqConnection.CloseAsync();
    }

    private async Task tickets_are_available_for_the_event()
    {
        Thread.Sleep(5000);
        var response = await client.GetAsync(EventTickets(event_id));
        response_code = response.StatusCode;
        content = response.Content;
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(await content.ReadAsStringAsync());
        ticket_ids = tickets.Select(t => t.Id).ToArray();
    }
    
    private async Task the_user_purchases_tickets_for_the_event()
    {
        content = new StringContent(
            JsonSerialization.Serialize(new TicketPurchasePayload(user_id, ticket_ids.Take(2).ToArray())),
            Encoding.UTF8,
            application_json);

        var response = await client.PostAsync(EventTickets(event_id) + "/purchase", content);
        response_code = response.StatusCode;
        content = response.Content;
    }

    private async Task the_purchase_is_successful()
    {
        response_code.ShouldBe(HttpStatusCode.NoContent);
        var response = await client.GetAsync(EventTickets(event_id));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(await response.Content.ReadAsStringAsync());
        tickets.Count.ShouldBe(15);
        foreach (var ticket in tickets.Where(t => ticket_ids.Take(2).Contains(t.Id)).ToList())
        {
            ticket.Purchased.ShouldBeTrue();
        }
    }
}