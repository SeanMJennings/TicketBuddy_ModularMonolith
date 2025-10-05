using System.Net;
using System.Text;
using BDD;
using Controllers.Tickets;
using Controllers.Tickets.Requests;
using Domain.Primitives;
using Integration.Events.Messaging;
using Integration.Tickets.Messaging.Messages;
using Integration.Users.Messaging;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using Shouldly;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using WebHost;

namespace Integration.Api;

public partial class TicketApiSpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private HttpContent content = null!;

    private Guid event_id = Guid.NewGuid();
    private Guid user_id = Guid.NewGuid();
    private readonly Guid another_user_id = Guid.NewGuid();
    private const decimal price = 25.00m;
    private const decimal new_price = 26.00m;
    private HttpStatusCode response_code;
    private const string application_json = "application/json";
    private const string name = "wibble";
    private const string full_name = "John Smith";
    private const string another_full_name = "Johnny Smith";
    private const string email = "john.smith@gmail.com";
    private const string another_email = "johnny.smith@gmail.com";
    private readonly DateTime event_start_date = DateTime.Now.AddDays(1);
    private readonly DateTime event_end_date = DateTime.Now.AddDays(1).AddHours(2);
    private static PostgreSqlContainer database = null!;
    private static RedisContainer redis = null!;
    private ITestHarness testHarness = null!;
    private Guid[] ticket_ids = null!;
    private static string TicketsForUser(Guid userId) => $"tickets/users/{userId}";
    private static string EventTickets(Guid id) => $"{Routes.Events}/{id}/tickets";

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
        redis = new RedisBuilder().WithPortBinding(6379, true).Build();
        redis.StartAsync().Await();
    }
    
    protected override void before_each()
    {
        base.before_each();
        content = null!;
        ticket_ids = [];
        event_id = Guid.NewGuid();
        user_id = Guid.NewGuid();
        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString(), redis.GetConnectionString());
        client = factory.CreateClient();
        testHarness = factory.Services.GetRequiredService<ITestHarness>();
        testHarness.Start().Await();
    }

    protected override void after_each()
    {
        Truncate(database.GetConnectionString());
        ClearRedisCache();
        testHarness.Stop().Await();
        client.Dispose();
        factory.Dispose();
    }

    private void ClearRedisCache()
    {
        redis.ExecScriptAsync("return redis.call('FLUSHALL')").Await();
    }

    protected override void after_all()
    {
        database.StopAsync().Await();
        database.DisposeAsync().GetAwaiter().GetResult();
        redis.StopAsync().Await();
        redis.DisposeAsync().GetAwaiter().GetResult();
    }

    private void an_event_exists()
    {
        testHarness.Bus.Publish(new EventUpserted
        {
            Id = event_id,
            EventName = name,
            StartDate = event_start_date,
            EndDate = event_end_date,
            Venue = Venue.EmiratesOldTraffordManchester,
            Price = price
        });
        testHarness.Consumed.Any<EventUpserted>(x => x.Context.Message.Id == event_id).Await();
    }

    private void a_user_exists()
    {
        testHarness.Bus.Publish(new UserUpserted
        {
            Id = user_id,
            FullName = full_name,
            Email = email
        });
        testHarness.Consumed.Any<UserUpserted>(x => x.Context.Message.Id == user_id).Await();
    }

    private void another_user_exists()
    {
        testHarness.Bus.Publish(new UserUpserted
        {
            Id = another_user_id,
            FullName = another_full_name,
            Email = another_email
        });
        testHarness.Consumed.Any<UserUpserted>(x => x.Context.Message.Id == another_user_id).Await();
    }

    private void requesting_the_tickets()
    {
        var response = client.GetAsync(EventTickets(event_id)).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
        var tickets = JsonSerialization.Deserialize<IList<Domain.Tickets.Queries.Ticket>>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        ticket_ids = tickets.Select(t => t.Id).ToArray();
    }

    private void purchasing_two_tickets()
    {
        content = new StringContent(
            JsonSerialization.Serialize(new TicketPurchasePayload(user_id, ticket_ids.Take(2).ToArray())),
            Encoding.UTF8,
            application_json);
        var response = client.PostAsync(EventTickets(event_id) + "/purchase", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }

    private void two_tickets_are_purchased()
    {
        purchasing_two_tickets();
    }
    
    private void purchasing_two_tickets_again()
    {
        purchasing_two_tickets();
    }
    
    private void reserving_a_ticket()
    {
        content = new StringContent(
            JsonSerialization.Serialize(new TicketReservationPayload(user_id, ticket_ids.Take(1).ToArray())),
            Encoding.UTF8,
            application_json);
        var response = client.PostAsync(EventTickets(event_id) + "/reserve", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }

    private void the_user_extends_their_reservation()
    {
        reserving_a_ticket();
    }

    private void another_user_reserving_a_ticket()
    {
        content = new StringContent(
            JsonSerialization.Serialize(new TicketReservationPayload(another_user_id, ticket_ids.Take(1).ToArray())),
            Encoding.UTF8,
            application_json);
        var response = client.PostAsync(EventTickets(event_id) + "/reserve", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }

    private void the_user_purchases_their_reserved_ticket()
    {
        purchasing_two_tickets();
    }

    private void another_user_purchasing_the_reserved_ticket()
    {
        content = new StringContent(
            JsonSerialization.Serialize(new TicketPurchasePayload(another_user_id, ticket_ids.Take(1).ToArray())),
            Encoding.UTF8,
            application_json);
        var response = client.PostAsync(EventTickets(event_id) + "/purchase", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }

    private void purchasing_two_non_existent_tickets()
    {
        content = new StringContent(
            JsonSerialization.Serialize(new TicketPurchasePayload(user_id, [Guid.NewGuid(), Guid.NewGuid()])),
            Encoding.UTF8,
            application_json);
        var response = client.PostAsync(EventTickets(event_id) + "/purchase", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }

    private void updating_the_ticket_prices()
    {
        testHarness.Bus.Publish(new EventUpserted
        {
            Id = event_id,
            EventName = name,
            StartDate = event_start_date,
            EndDate = event_end_date,
            Venue = Venue.EmiratesOldTraffordManchester,
            Price = new_price
        });
        testHarness.Consumed.Any<EventUpserted>(x => x.Context.Message.Id == event_id && x.Context.Message.Price == new_price).Await();
    }

    private void the_tickets_are_released()
    {
        response_code.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(content.ReadAsStringAsync().GetAwaiter().GetResult());
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

    private void the_tickets_are_purchased()
    {
        response_code.ShouldBe(HttpStatusCode.NoContent);
        var response = client.GetAsync(EventTickets(event_id)).GetAwaiter().GetResult();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        tickets.Count.ShouldBe(17);
        foreach (var ticket in tickets.Where(t => ticket_ids.Take(2).Contains(t.Id)).ToList())
        {
            ticket.Purchased.ShouldBeTrue();
        }
    }

    private void user_informed_they_cannot_purchase_tickets_that_are_purchased()
    {
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        var theError = JsonSerialization.Deserialize<ApiError>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        theError.Errors.ShouldContain("Tickets are not available");
    }

    private void user_informed_they_cannot_purchase_tickets_that_are_non_existent()
    {
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        var theError = JsonSerialization.Deserialize<ApiError>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        theError.Errors.ShouldContain("One or more tickets do not exist");
    }

    private void the_ticket_prices_are_updated()
    {
        response_code.ShouldBe(HttpStatusCode.NoContent);
        var response = client.GetAsync(EventTickets(event_id)).GetAwaiter().GetResult();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        tickets.Count.ShouldBe(17);
        foreach (var ticket in tickets.Where(t => !ticket_ids.Take(2).Contains(t.Id)).ToList())
        {
            ticket.Price.ShouldBe(new_price);
        }
    }

    private void purchased_tickets_are_not_updated()
    {
        var response = client.GetAsync(TicketsForUser(user_id)).GetAwaiter().GetResult();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        tickets.Count.ShouldBe(2);
        foreach (var ticket in tickets)
        {
            ticket.Price.ShouldBe(price);
        }
    }

    private void the_ticket_is_reserved()
    {
        response_code.ShouldBe(HttpStatusCode.NoContent);
        var response = client.GetAsync(EventTickets(event_id)).GetAwaiter().GetResult();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(response.Content.ReadAsStringAsync().Await());
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

    private void user_informed_they_cannot_reserve_an_already_reserved_ticket()
    {
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        var theError = JsonSerialization.Deserialize<ApiError>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        theError.Errors.ShouldContain("Tickets already reserved");
    }

    private void another_user_informed_they_cannot_purchase_a_reserved_ticket()
    {
        response_code.ShouldBe(HttpStatusCode.BadRequest);
        var theError = JsonSerialization.Deserialize<ApiError>(content.ReadAsStringAsync().GetAwaiter().GetResult());
        theError.Errors.ShouldContain("Tickets already reserved");
    }
    
    private void purchasing_all_tickets()
    {
        content = new StringContent(
            JsonSerialization.Serialize(new TicketPurchasePayload(user_id, ticket_ids)),
            Encoding.UTF8,
            application_json);
        var response = client.PostAsync(EventTickets(event_id) + "/purchase", content).GetAwaiter().GetResult();
        response_code = response.StatusCode;
        content = response.Content;
    }
    
    private void event_sold_out_integration_event_is_published()
    {
        response_code.ShouldBe(HttpStatusCode.NoContent);
        
        var response = client.GetAsync(EventTickets(event_id)).GetAwaiter().GetResult();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tickets = JsonSerialization.Deserialize<IList<Ticket>>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        tickets.Count.ShouldBe(17);
        tickets.All(t => t.Purchased).ShouldBeTrue();
        
        testHarness.Published.Any<EventSoldOut>(x => x.Context.Message.EventId == event_id).Await().ShouldBeTrue();
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