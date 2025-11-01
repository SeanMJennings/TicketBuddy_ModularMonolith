using System.Net;
using System.Text;
using BDD;
using Component;
using Controllers.Events;
using Controllers.Events.Requests;
using Controllers.Tickets.Requests;
using Controllers.Users.Requests;
using Domain.Primitives;
using Domain.Tickets.Queries;
using Domain.Users.Primitives;
using Migrations;
using Shouldly;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using UserRoutes = Controllers.Users.Routes;

namespace Acceptance;

public partial class TicketBuddySpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private HttpContent content = null!;

    private Guid event_id;
    private Guid user_id;
    private HttpStatusCode response_code;
    private const string application_json = "application/json";
    private const string name = "wibble";
    private const string email = "wibble@wobble.com";
    private readonly DateTimeOffset event_start_date = DateTimeOffset.UtcNow.AddDays(3);
    private readonly DateTimeOffset event_end_date = DateTimeOffset.UtcNow.AddDays(3).AddHours(2);
    private const decimal price = 12.34m;
    private static PostgreSqlContainer database = null!;
    private static RabbitMqContainer rabbit = null!;
    private static RedisContainer redis = null!;
    private Guid[] ticket_ids = [];
    private static string EventTickets(Guid id) => $"{Routes.Events}/{id}/tickets";

    protected override async Task before_all()
    {
        database = new PostgreSqlBuilder()
            .WithDatabase("TicketBuddy")
            .WithUsername("sa")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1435)
            .WithReuse(true)
            .Build();
        await database.StartAsync();
        rabbit = new RabbitMqBuilder()
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(5674)
            .WithReuse(true)
            .Build();
        await rabbit.StartAsync();
        redis = new RedisBuilder()
            .WithPortBinding(6381)
            .WithReuse(true)
            .Build();
        await redis.StartAsync();
        Migration.Upgrade(database.GetConnectionString());
    }
    
    protected override Task before_each()
    {
        content = null!;
        user_id = Guid.Empty;
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
    }
    
    private async Task an_event_exists()
    {
        var theContent = new StringContent(
            JsonSerialization.Serialize(new EventPayload(name, event_start_date, event_end_date, Venue.FirstDirectArenaLeeds, price)),
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
        var theContent = new StringContent(
            JsonSerialization.Serialize(new UserPayload(name, email, UserType.Customer)),
            Encoding.UTF8, 
            application_json);

        var response = await client.PostAsync(UserRoutes.Users, theContent);
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.Created);
        user_id = JsonSerialization.Deserialize<Guid>(await response.Content.ReadAsStringAsync());
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