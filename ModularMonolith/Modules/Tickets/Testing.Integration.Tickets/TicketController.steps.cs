using System.ComponentModel.DataAnnotations;
using Application.Tickets.IntegrationMessageConsumers;
using BDD;
using Controllers.Tickets;
using Controllers.Tickets.Requests;
using Infrastructure.Configuration;
using Infrastructure.Tickets.Configuration;
using Integration.Events.Messaging;
using Integration.Keycloak.Users.Messaging;
using Integration.Tickets.Messaging.Messages;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Testing;
using Testing.Containers;
using Venue = Domain.Primitives.Venue;

namespace Integration;

public partial class TicketControllerSpecs : TruncateDbSpecification
{
    private TicketController ticketController = null!;
    private EventConsumer eventConsumer = null!;
    private UserRegisteredConsumer userRegisteredConsumer = null!;
    private ServiceProvider serviceProvider = null!;
    private StackExchange.Redis.IConnectionMultiplexer cache = null!;
    private Exception theError = null!;

    private Guid event_id = Guid.NewGuid();
    private Guid user_id = Guid.NewGuid();
    private readonly Guid another_user_id = Guid.NewGuid();
    private const decimal price = 25.00m;
    private const decimal new_price = 26.00m;
    private const string name = "wibble";
    private const string another_full_name = "Johnny Smith";
    private const string email = "john.smith@gmail.com";
    private const string another_email = "johnny.smith@gmail.com";
    private readonly DateTime event_start_date = DateTime.Now.AddDays(1);
    private readonly DateTime event_end_date = DateTime.Now.AddDays(1).AddHours(2);
    private static PostgreSqlContainer database = null!;
    private static RedisContainer redis = null!;
    private ITestHarness testHarness = null!;
    private Guid[] ticket_ids = null!;

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        database.Migrate();
        redis = Redis.CreateContainer();
        await redis.StartAsync();
    }
    
    protected override Task before_each()
    {
        ticket_ids = [];
        event_id = Guid.NewGuid();
        user_id = Guid.NewGuid();
        theError = null!;
        
        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureCache(redis.GetConnectionString())
            .ConfigureTicketsDatabase(database.GetConnectionString())
            .AddMassTransitTestHarness(x =>
            {
                x.AddTicketsConsumers();
            })
            .AddSingleton(new Dictionary<Type, Type>())
            .ConfigureTicketsServices()
            .AddScoped<TicketController>()
            .BuildServiceProvider();
        
        testHarness = serviceProvider.GetRequiredService<ITestHarness>();
        testHarness.Start().Await();
        ticketController = serviceProvider.GetRequiredService<TicketController>();
        cache = serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
        eventConsumer = serviceProvider.GetRequiredService<EventConsumer>();
        userRegisteredConsumer = serviceProvider.GetRequiredService<UserRegisteredConsumer>();
        return Task.CompletedTask;
    }

    protected override async Task after_each()
    {
        await Truncate(database.GetConnectionString());
        await ClearRedisCache();
        await testHarness.Stop();
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
        // would prefer to use the test harness here but haven't got it working yet
        var mockContext = Substitute.For<ConsumeContext<EventUpserted>>();
        mockContext.Message.Returns(new EventUpserted
        {
            Id = event_id,
            EventName = name,
            StartDate = event_start_date,
            EndDate = event_end_date,
            Venue = Venue.EmiratesOldTraffordManchester,
            Price = price
        });
        await eventConsumer.Consume(mockContext);
    }

    private async Task a_user_exists()
    {
        // would prefer to use the test harness here but haven't got it working yet
        var mockContext = Substitute.For<ConsumeContext<UserRegistered>>();
        var details = new Dictionary<string, string>
        {
            { "first_name", "John" },
            { "last_name", "Smith" },
            { "email", email }
        };
        mockContext.Message.Returns(new UserRegistered(user_id, details));
        await userRegisteredConsumer.Consume(mockContext);
    }

    private async Task another_user_exists()
    {
        // would prefer to use the test harness here but haven't got it working yet
        var mockContext = Substitute.For<ConsumeContext<UserRegistered>>();
        var details = new Dictionary<string, string>
        {
            { "first_name", "Johnny" },
            { "last_name", "Smith" },
            { "email", another_email }
        };
        mockContext.Message.Returns(new UserRegistered(another_user_id, details));
        await userRegisteredConsumer.Consume(mockContext);
    }

    private async Task requesting_the_tickets()
    {
        var tickets = await ticketController.GetTickets(event_id);
        ticket_ids = tickets.Select(t => t.Id).ToArray();
    }

    private async Task purchasing_two_tickets()
    {
        var payload = new TicketPurchasePayload(user_id, ticket_ids.Take(2).ToArray());
        await ticketController.PurchaseTickets(event_id, payload);
    }

    private async Task two_tickets_are_purchased()
    {
        await purchasing_two_tickets();
    }
    
    private async Task purchasing_two_tickets_again()
    {
        try
        {
            await purchasing_two_tickets();
        }
        catch (ValidationException ex)
        {
            theError = ex;
        }
    }
    
    private async Task reserving_a_ticket()
    {
        var payload = new TicketReservationPayload(user_id, ticket_ids.Take(1).ToArray());
        await ticketController.ReserveTickets(event_id, payload);
    }

    private async Task the_user_extends_their_reservation()
    {
        await reserving_a_ticket();
    }

    private async Task another_user_reserving_a_ticket()
    {
        var payload = new TicketReservationPayload(another_user_id, ticket_ids.Take(1).ToArray());
        try
        {
            await ticketController.ReserveTickets(event_id, payload);
        }
        catch (ValidationException ex)
        {
            theError = ex;
        }
    }

    private async Task the_user_purchases_their_reserved_ticket()
    {
        await purchasing_two_tickets();
    }

    private async Task another_user_purchasing_the_reserved_ticket()
    {
        var payload = new TicketPurchasePayload(another_user_id, ticket_ids.Take(1).ToArray());
        try
        {
            await ticketController.PurchaseTickets(event_id, payload);
        }
        catch (ValidationException ex)
        {
            theError = ex;
        }
    }

    private async Task purchasing_two_non_existent_tickets()
    {
        var payload = new TicketPurchasePayload(user_id, [Guid.NewGuid(), Guid.NewGuid()]);
        try
        {
            await ticketController.PurchaseTickets(event_id, payload);
        }
        catch (Exception ex)
        {
            theError = ex;
        }
    }

    private async Task updating_the_ticket_prices()
    {
        // would prefer to use the test harness here but haven't got it working yet
        var mockContext = Substitute.For<ConsumeContext<EventUpserted>>();
        mockContext.Message.Returns(new EventUpserted
        {
            Id = event_id,
            EventName = name,
            StartDate = event_start_date,
            EndDate = event_end_date,
            Venue = Venue.EmiratesOldTraffordManchester,
            Price = new_price
        });
        await eventConsumer.Consume(mockContext);
    }

    private async Task the_tickets_are_released()
    {
        var tickets = await ticketController.GetTickets(event_id);
        tickets.Count.ShouldBe(17);
        tickets = tickets.OrderBy(t => t.SeatNumber).ToList();
        var counter = 1;
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
        var tickets = await ticketController.GetTickets(event_id);
        tickets.Count.ShouldBe(17);
        foreach (var ticket in tickets.Where(t => ticket_ids.Take(2).Contains(t.Id)).ToList())
        {
            ticket.Purchased.ShouldBeTrue();
        }
    }

    private void user_informed_they_cannot_purchase_tickets_that_are_purchased()
    {
        theError.Message.ShouldContain("Tickets are not available");
    }

    private void user_informed_they_cannot_purchase_tickets_that_are_non_existent()
    {
        theError.Message.ShouldContain("One or more tickets do not exist");
    }

    private async Task the_ticket_prices_are_updated()
    {
        var tickets = await ticketController.GetTickets(event_id);
        tickets.Count.ShouldBe(17);
        foreach (var ticket in tickets.Where(t => !ticket_ids.Take(2).Contains(t.Id)).ToList())
        {
            ticket.Price.ShouldBe(new_price);
        }
    }

    private async Task purchased_tickets_are_not_updated()
    {
        var tickets = await ticketController.GetTicketsForUser(user_id);
        tickets.Count.ShouldBe(2);
        foreach (var ticket in tickets)
        {
            ticket.Price.ShouldBe(price);
        }
    }

    private async Task the_ticket_is_reserved()
    {
        var tickets = await ticketController.GetTickets(event_id);
        tickets.Count.ShouldBe(17);
        var reservedTicket = tickets.Single(t => t.Id == ticket_ids.Take(1).First());
        reservedTicket.Reserved.ShouldBeTrue();
    }

    private void the_reservation_expires_in_15_minutes()
    {
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
        theError.Message.ShouldContain("Tickets already reserved");
    }

    private void another_user_informed_they_cannot_purchase_a_reserved_ticket()
    {
        theError.Message.ShouldContain("Tickets already reserved");
    }
    
    private async Task purchasing_all_tickets()
    {
        var payload = new TicketPurchasePayload(user_id, ticket_ids);
        await ticketController.PurchaseTickets(event_id, payload);
    }
    
    private async Task event_sold_out_integration_event_is_published()
    {
        var tickets = await ticketController.GetTickets(event_id);
        tickets.Count.ShouldBe(17);
        tickets.All(t => t.Purchased).ShouldBeTrue();
        
        testHarness.Published.Any<EventSoldOut>(x => x.Context.Message.EventId == event_id).Await().ShouldBeTrue();
    }
}