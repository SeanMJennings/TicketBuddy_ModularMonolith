using System.ComponentModel.DataAnnotations;
using Application.Tickets.IntegrationMessageConsumers;
using BDD;
using Controllers.Tickets;
using Controllers.Tickets.Requests;
using Infrastructure.Configuration;
using Infrastructure.Tickets.Configuration;
using Integration.Events.Messaging;
using Integration.Tickets.Messaging.Messages;
using Integration.Users.Messaging;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Migrations;
using NSubstitute;
using Shouldly;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Venue = Domain.Primitives.Venue;

namespace Integration;

public partial class TicketControllerSpecs : TruncateDbSpecification
{
    private TicketController ticketController = null!;
    private EventConsumer eventConsumer = null!;
    private UserConsumer userConsumer = null!;
    private ServiceProvider serviceProvider = null!;
    private StackExchange.Redis.IConnectionMultiplexer cache = null!;
    private Exception theError = null!;

    private Guid event_id = Guid.NewGuid();
    private Guid user_id = Guid.NewGuid();
    private readonly Guid another_user_id = Guid.NewGuid();
    private const decimal price = 25.00m;
    private const decimal new_price = 26.00m;
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

    protected override void before_all()
    {
        database = new PostgreSqlBuilder()
            .WithDatabase("TicketBuddy")
            .WithUsername("sa")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1434, true)
            .Build();
        database.StartAsync().Await();
        Migration.Upgrade(database.GetConnectionString());
        redis = new RedisBuilder().WithPortBinding(6380, true).Build();
        redis.StartAsync().Await();
    }
    
    protected override void before_each()
    {
        base.before_each();
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
        userConsumer = serviceProvider.GetRequiredService<UserConsumer>();
    }

    protected override void after_each()
    {
        Truncate(database.GetConnectionString());
        ClearRedisCache();
        testHarness.Stop().Await();
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
        eventConsumer.Consume(mockContext).Await();
    }

    private void a_user_exists()
    {
        // would prefer to use the test harness here but haven't got it working yet
        var mockContext = Substitute.For<ConsumeContext<UserUpserted>>();
        mockContext.Message.Returns(new UserUpserted
        {
            Id = user_id,
            FullName = full_name,
            Email = email
        });
        userConsumer.Consume(mockContext).Await();
    }

    private void another_user_exists()
    {
        // would prefer to use the test harness here but haven't got it working yet
        var mockContext = Substitute.For<ConsumeContext<UserUpserted>>();
        mockContext.Message.Returns(new UserUpserted
        {
            Id = another_user_id,
            FullName = another_full_name,
            Email = another_email
        });
        userConsumer.Consume(mockContext).Await();
    }

    private void requesting_the_tickets()
    {
        var tickets = ticketController.GetTickets(event_id).Await();
        ticket_ids = tickets.Select(t => t.Id).ToArray();
    }

    private void purchasing_two_tickets()
    {
        var payload = new TicketPurchasePayload(user_id, ticket_ids.Take(2).ToArray());
        ticketController.PurchaseTickets(event_id, payload).Await();
    }

    private void two_tickets_are_purchased()
    {
        purchasing_two_tickets();
    }
    
    private void purchasing_two_tickets_again()
    {
        try
        {
            purchasing_two_tickets();
        }
        catch (ValidationException ex)
        {
            theError = ex;
        }
    }
    
    private void reserving_a_ticket()
    {
        var payload = new TicketReservationPayload(user_id, ticket_ids.Take(1).ToArray());
        ticketController.ReserveTickets(event_id, payload).Await();
    }

    private void the_user_extends_their_reservation()
    {
        reserving_a_ticket();
    }

    private void another_user_reserving_a_ticket()
    {
        var payload = new TicketReservationPayload(another_user_id, ticket_ids.Take(1).ToArray());
        try
        {
            ticketController.ReserveTickets(event_id, payload).Await();
        }
        catch (ValidationException ex)
        {
            theError = ex;
        }
    }

    private void the_user_purchases_their_reserved_ticket()
    {
        purchasing_two_tickets();
    }

    private void another_user_purchasing_the_reserved_ticket()
    {
        var payload = new TicketPurchasePayload(another_user_id, ticket_ids.Take(1).ToArray());
        try
        {
            ticketController.PurchaseTickets(event_id, payload).Await();
        }
        catch (ValidationException ex)
        {
            theError = ex;
        }
    }

    private void purchasing_two_non_existent_tickets()
    {
        var payload = new TicketPurchasePayload(user_id, [Guid.NewGuid(), Guid.NewGuid()]);
        try
        {
            ticketController.PurchaseTickets(event_id, payload).Await();
        }
        catch (Exception ex)
        {
            theError = ex;
        }
    }

    private void updating_the_ticket_prices()
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
        eventConsumer.Consume(mockContext).Await();
    }

    private void the_tickets_are_released()
    {
        var tickets = ticketController.GetTickets(event_id).Await();
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

    private void the_tickets_are_purchased()
    {
        var tickets = ticketController.GetTickets(event_id).Await();
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

    private void the_ticket_prices_are_updated()
    {
        var tickets = ticketController.GetTickets(event_id).Await();
        tickets.Count.ShouldBe(17);
        foreach (var ticket in tickets.Where(t => !ticket_ids.Take(2).Contains(t.Id)).ToList())
        {
            ticket.Price.ShouldBe(new_price);
        }
    }

    private void purchased_tickets_are_not_updated()
    {
        var tickets = ticketController.GetTicketsForUser(user_id).Await();
        tickets.Count.ShouldBe(2);
        foreach (var ticket in tickets)
        {
            ticket.Price.ShouldBe(price);
        }
    }

    private void the_ticket_is_reserved()
    {
        var tickets = ticketController.GetTickets(event_id).Await();
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
    
    private void purchasing_all_tickets()
    {
        var payload = new TicketPurchasePayload(user_id, ticket_ids);
        ticketController.PurchaseTickets(event_id, payload).Await();
    }
    
    private void event_sold_out_integration_event_is_published()
    {
        var tickets = ticketController.GetTickets(event_id).Await();
        tickets.Count.ShouldBe(17);
        tickets.All(t => t.Purchased).ShouldBeTrue();
        
        testHarness.Published.Any<EventSoldOut>(x => x.Context.Message.EventId == event_id).Await().ShouldBeTrue();
    }
}