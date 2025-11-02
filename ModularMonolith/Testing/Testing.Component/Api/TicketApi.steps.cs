using System.Net;
using System.Text;
using Controllers.Tickets;
using Controllers.Tickets.Requests;
using Domain.Primitives;
using Integration.Events.Messaging;
using Integration.Users.Messaging;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Testing;
using Testing.Containers;

namespace Component.Api;

public partial class TicketApiSpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private HttpContent content = null!;

    private Guid event_id = Guid.NewGuid();
    private Guid user_id = Guid.NewGuid();
    private const decimal price = 25.00m;
    private const decimal new_price = 26.00m;
    private HttpStatusCode response_code;
    private const string application_json = "application/json";
    private const string name = "wibble";
    private const string full_name = "John Smith";
    private const string email = "john.smith@gmail.com";
    private readonly DateTime event_start_date = DateTime.Now.AddDays(1);
    private readonly DateTime event_end_date = DateTime.Now.AddDays(1).AddHours(2);
    private static PostgreSqlContainer database = null!;
    private static RedisContainer redis = null!;
    private ITestHarness testHarness = null!;
    private Guid[] ticket_ids = null!;
    private static string TicketsForUser(Guid userId) => $"tickets/users/{userId}";
    private static string EventTickets(Guid id) => $"{Routes.Events}/{id}/tickets";

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        database.Migrate();
        redis = Redis.CreateContainer();
        await redis.StartAsync();
    }
    
    protected override async Task before_each()
    {
        content = null!;
        ticket_ids = [];
        event_id = Guid.NewGuid();
        user_id = Guid.NewGuid();
        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString(), redis.GetConnectionString());
        client = factory.CreateClient();
        testHarness = factory.Services.GetRequiredService<ITestHarness>();
        await testHarness.Start();
    }

    protected override async Task after_each()
    {
        await Truncate(database.GetConnectionString());
        await ClearRedisCache();
        await testHarness.Stop();
        client.Dispose();
        await factory.DisposeAsync();
    }

    private async Task ClearRedisCache()
    {
        await redis.Clear();
    }

    protected override async Task after_all()
    {
        await database.StopAsync();
        await database.DisposeAsync();
        await redis.StopAsync();
        await redis.DisposeAsync();
    }

    private async Task an_event_exists()
    {
        await testHarness.Bus.Publish(new EventUpserted
        {
            Id = event_id,
            EventName = name,
            StartDate = event_start_date,
            EndDate = event_end_date,
            Venue = Venue.EmiratesOldTraffordManchester,
            Price = price
        });
        await testHarness.Consumed.Any<EventUpserted>(x => x.Context.Message.Id == event_id);
    }

    private async Task a_user_exists()
    {
        await testHarness.Bus.Publish(new UserUpserted
        {
            Id = user_id,
            FullName = full_name,
            Email = email
        });
        await testHarness.Consumed.Any<UserUpserted>(x => x.Context.Message.Id == user_id);
    }

    private async Task requesting_the_tickets()
    {
        var response = await client.GetAsync(EventTickets(event_id));
        response_code = response.StatusCode;
        content = response.Content;
        var tickets = JsonSerialization.Deserialize<IList<Domain.Tickets.Queries.Ticket>>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        ticket_ids = tickets.Select(t => t.Id).ToArray();
    }

    private async Task purchasing_two_tickets()
    {
        content = new StringContent(
            JsonSerialization.Serialize(new TicketPurchasePayload(user_id, ticket_ids.Take(2).ToArray())),
            Encoding.UTF8,
            application_json);
        var response = await client.PostAsync(EventTickets(event_id) + "/purchase", content);
        response_code = response.StatusCode;
        content = response.Content;
    }

    private async Task two_tickets_are_purchased()
    {
        await purchasing_two_tickets();
    }
    
    private async Task reserving_a_ticket()
    {
        content = new StringContent(
            JsonSerialization.Serialize(new TicketReservationPayload(user_id, ticket_ids.Take(1).ToArray())),
            Encoding.UTF8,
            application_json);
        var response = await client.PostAsync(EventTickets(event_id) + "/reserve", content);
        response_code = response.StatusCode;
        content = response.Content;
    }

    private async Task updating_the_ticket_prices()
    {
        await testHarness.Bus.Publish(new EventUpserted
        {
            Id = event_id,
            EventName = name,
            StartDate = event_start_date,
            EndDate = event_end_date,
            Venue = Venue.EmiratesOldTraffordManchester,
            Price = new_price
        });
        await testHarness.Consumed.Any<EventUpserted>(x => x.Context.Message.Id == event_id && x.Context.Message.Price == new_price);
    }

    private async Task the_tickets_are_released()
    {
        response_code.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(await content.ReadAsStringAsync());
        tickets.Count.ShouldBe(17);
        tickets = tickets.OrderBy(t => t.SeatNumber).ToList();
        uint counter = 1;
        foreach (var ticket in tickets)
        {
            ticket.EventId.ShouldBe(event_id);
            ticket.Price.ShouldBe(price);
            ticket.SeatNumber.ShouldBe(counter);
            counter++;
        }
    }

    private async Task the_tickets_are_purchased()
    {
        response_code.ShouldBe(HttpStatusCode.NoContent);
        var response = await client.GetAsync(EventTickets(event_id));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(await response.Content.ReadAsStringAsync());
        tickets.Count.ShouldBe(17);
        foreach (var ticket in tickets.Where(t => ticket_ids.Take(2).Contains(t.Id)).ToList())
        {
            ticket.Purchased.ShouldBeTrue();
        }
    }

    private async Task the_ticket_prices_are_updated()
    {
        response_code.ShouldBe(HttpStatusCode.NoContent);
        var response = await client.GetAsync(EventTickets(event_id));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(await response.Content.ReadAsStringAsync());
        tickets.Count.ShouldBe(17);
        foreach (var ticket in tickets.Where(t => !ticket_ids.Take(2).Contains(t.Id)).ToList())
        {
            ticket.Price.ShouldBe(new_price);
        }
    }

    private async Task purchased_tickets_are_not_updated()
    {
        var response = await client.GetAsync(TicketsForUser(user_id));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(await response.Content.ReadAsStringAsync());
        tickets.Count.ShouldBe(2);
        foreach (var ticket in tickets)
        {
            ticket.Price.ShouldBe(price);
        }
    }

    private async Task the_ticket_is_reserved()
    {
        response_code.ShouldBe(HttpStatusCode.NoContent);
        var response = await client.GetAsync(EventTickets(event_id));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(await response.Content.ReadAsStringAsync());
        tickets.Count.ShouldBe(17);
        var reservedTicket = tickets.Single(t => t.Id == ticket_ids.Take(1).First());
        reservedTicket.Reserved.ShouldBeTrue();
    }

    private void the_reservation_expires_in_15_minutes()
    {
        var cache = factory.Services.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
        var db = cache.GetDatabase();
        var reservationKey = $"event:{event_id}:ticket:{ticket_ids.Take(1).First()}:reservation";
        var ttl = db.KeyTimeToLive(reservationKey);
        ttl.HasValue.ShouldBeTrue();
        ttl!.Value.TotalMinutes.ShouldBeLessThanOrEqualTo(15);
        ttl.Value.TotalMinutes.ShouldBeGreaterThan(14);
        var keyValue = db.StringGet(reservationKey);
        keyValue.HasValue.ShouldBeTrue();
        keyValue.ToString().ShouldBe(user_id.ToString());
    }
    
    // [NotMapped] property in read model class affects serialization, so using a private class here for testing
    private class Ticket
    {
        public Guid Id { get; init; }
        public Guid EventId { get; init; }
        public decimal Price { get; init; }
        public uint SeatNumber { get; init; }
        public bool Purchased { get; init; }
        public bool Reserved { get; init; }
    }
}