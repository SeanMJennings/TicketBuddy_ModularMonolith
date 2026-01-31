using System.Security.Claims;
using Controllers.Tickets.Requests;
using Controllers.Tickets.Ticket;
using Infrastructure.Configuration;
using Infrastructure.Tickets.Configuration;
using Infrastructure.Tickets.Core.Configuration;
using MassTransit;
using MassTransit.Testing;
using Messages.Events;
using Messages.Tickets;
using Messaging.Keycloak.Users;
using Messaging.Tickets.Consumers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Testing;
using Testing.Containers;

namespace Integration.Tickets;

public partial class PurchaseTicketsSpecs : TruncateDbSpecification
{
    private GetTicketsForEventEndpoint getTicketsForEventEndpoint = null!;
    private GetTicketsForUserEndpoint getTicketsForUserEndpoint = null!;
    private PurchaseTicketsEndpoint purchaseTicketsEndpoint = null!;
    private ReserveTicketsEndpoint reserveTicketsEndpoint = null!;
    private EventUpsertedConsumer eventUpsertedConsumer = null!;
    private VenueUpsertedConsumer venueUpsertedConsumer = null!;
    private UserRegisteredConsumer userRegisteredConsumer = null!;
    private ServiceProvider serviceProvider = null!;
    private Guid nonExistentEventId = Guid.NewGuid();
    private ITestHarness testHarness = null!;

    private Guid event_id = Guid.NewGuid();
    private Guid user_id = Guid.NewGuid();
    private readonly Guid another_user_id = Guid.NewGuid();
    private const decimal price = 25.00m;
    private const decimal new_price = 26.00m;
    private const string name = "wibble";
    private const string email = "john.smith@gmail.com";
    private const string another_email = "johnny.smith@gmail.com";
    private readonly DateTime event_start_date = DateTime.Now.AddDays(1);
    private readonly DateTime event_end_date = DateTime.Now.AddDays(1).AddHours(2);
    private Guid[] ticket_ids = null!;

    protected override Task before_each()
    {
        ticket_ids = [];
        event_id = Guid.NewGuid();
        user_id = Guid.NewGuid();

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureCache(Setup.Redis.GetConnectionString())
            .ConfigureTicketsDatabase(Setup.Database.GetConnectionString())
            .AddMassTransitTestHarness(x =>
            {
                x.AddTicketsConsumers();
            })
            .AddSingleton(new Dictionary<Type, Type>())
            .ConfigureTicketsServices()
            .AddScoped<GetTicketsForEventEndpoint>()
            .AddScoped<GetTicketsForUserEndpoint>()
            .AddScoped<PurchaseTicketsEndpoint>()
            .AddScoped<ReserveTicketsEndpoint>()
            .BuildServiceProvider();

        testHarness = serviceProvider.GetRequiredService<ITestHarness>();
        testHarness.Start().GetAwaiter().GetResult();
        getTicketsForEventEndpoint = serviceProvider.GetRequiredService<GetTicketsForEventEndpoint>();
        getTicketsForUserEndpoint = serviceProvider.GetRequiredService<GetTicketsForUserEndpoint>();
        purchaseTicketsEndpoint = serviceProvider.GetRequiredService<PurchaseTicketsEndpoint>();
        reserveTicketsEndpoint = serviceProvider.GetRequiredService<ReserveTicketsEndpoint>();
        AddUserClaimToControllerContext(user_id);
        eventUpsertedConsumer = serviceProvider.GetRequiredService<EventUpsertedConsumer>();
        venueUpsertedConsumer = serviceProvider.GetRequiredService<VenueUpsertedConsumer>();
        userRegisteredConsumer = serviceProvider.GetRequiredService<UserRegisteredConsumer>();
        return Task.CompletedTask;
    }

    private void AddUserClaimToControllerContext(Guid userId)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", userId.ToString())], "TestAuth"));
        getTicketsForEventEndpoint.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        getTicketsForUserEndpoint.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        purchaseTicketsEndpoint.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
        reserveTicketsEndpoint.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    protected override async Task after_each()
    {
        await Truncate(Setup.Database.GetConnectionString());
        await Setup.Redis.Clear();
        await testHarness.Stop();
    }

    private async Task an_event_exists()
    {
        var venueId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var venueContext = Substitute.For<ConsumeContext<VenueUpserted>>();
        venueContext.Message.Returns(new VenueUpserted
        {
            Id = venueId,
            Name = "Test Venue",
            Capacity = 17
        });
        await venueUpsertedConsumer.Consume(venueContext);

        var eventContext = Substitute.For<ConsumeContext<EventUpserted>>();
        eventContext.Message.Returns(new EventUpserted
        {
            Id = event_id,
            EventName = name,
            StartDate = event_start_date,
            EndDate = event_end_date,
            VenueId = venueId,
            Price = price
        });
        await eventUpsertedConsumer.Consume(eventContext);
    }

    private async Task a_user_exists()
    {
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
        var tickets = await getTicketsForEventEndpoint.GetTickets(event_id);
        ticket_ids = tickets.Select(t => t.Id).ToArray();
    }

    private async Task purchasing_two_tickets()
    {
        var payload = new TicketPurchasePayload(ticket_ids.Take(2).ToArray());
        await purchaseTicketsEndpoint.PurchaseTickets(event_id, payload);
    }

    private async Task two_tickets_are_purchased()
    {
        await purchasing_two_tickets();
    }

    private async Task purchasing_two_tickets_again()
    {
        await purchasing_two_tickets();
    }

    private async Task reserving_tickets()
    {
        AddUserClaimToControllerContext(user_id);
        var payload = new TicketReservationPayload(ticket_ids.Take(2).ToArray());
        await reserveTicketsEndpoint.ReserveTickets(event_id, payload);
    }

    private async Task another_user_purchasing_the_reserved_tickets()
    {
        AddUserClaimToControllerContext(another_user_id);
        var payload = new TicketPurchasePayload(ticket_ids.Take(2).ToArray());
        await purchaseTicketsEndpoint.PurchaseTickets(event_id, payload);
    }

    private async Task the_user_purchases_their_reserved_tickets()
    {
        await purchasing_two_tickets();
    }    
    
    private async Task the_user_purchases_tickets_that_are_not_reserved()
    {
        await purchasing_two_tickets();
    }

    private async Task purchasing_two_non_existent_tickets()
    {
        AddUserClaimToControllerContext(user_id);
        var payload = new TicketPurchasePayload([Guid.NewGuid(), Guid.NewGuid()]);
        await purchaseTicketsEndpoint.PurchaseTickets(event_id, payload);
    }

    private async Task purchasing_tickets_for_non_existent_event()
    {
        AddUserClaimToControllerContext(user_id);
        nonExistentEventId = Guid.NewGuid();
        var payload = new TicketPurchasePayload([Guid.NewGuid()]);
        await purchaseTicketsEndpoint.PurchaseTickets(nonExistentEventId, payload);
    }

    private async Task updating_the_ticket_prices()
    {
        var mockContext = Substitute.For<ConsumeContext<EventUpserted>>();
        mockContext.Message.Returns(new EventUpserted
        {
            Id = event_id,
            EventName = name,
            StartDate = event_start_date,
            EndDate = event_end_date,
            VenueId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Price = new_price
        });
        await eventUpsertedConsumer.Consume(mockContext);
    }

    private async Task reserving_all_tickets()
    {
        AddUserClaimToControllerContext(user_id);
        var payload = new TicketReservationPayload(ticket_ids);
        await reserveTicketsEndpoint.ReserveTickets(event_id, payload);
    }

    private async Task purchasing_all_tickets()
    {
        AddUserClaimToControllerContext(user_id);
        var payload = new TicketPurchasePayload(ticket_ids);
        await purchaseTicketsEndpoint.PurchaseTickets(event_id, payload);
    }

    private async Task the_tickets_are_purchased()
    {
        var tickets = await getTicketsForEventEndpoint.GetTickets(event_id);
        tickets.Count.ShouldBe(17);
        foreach (var ticket in tickets.Where(t => ticket_ids.Take(2).Contains(t.Id)).ToList())
        {
            ticket.Purchased.ShouldBeTrue();
        }
    }

    private async Task the_ticket_prices_are_updated()
    {
        var tickets = await getTicketsForEventEndpoint.GetTickets(event_id);
        tickets.Count.ShouldBe(17);
        foreach (var ticket in tickets.Where(t => !ticket_ids.Take(2).Contains(t.Id)).ToList())
        {
            ticket.Price.ShouldBe(new_price);
        }
    }

    private async Task purchased_tickets_are_not_updated()
    {
        AddUserClaimToControllerContext(user_id);
        var tickets = await getTicketsForUserEndpoint.GetTicketsForUser();
        tickets.Count.ShouldBe(2);
        foreach (var ticket in tickets)
        {
            ticket.Price.ShouldBe(price);
        }
    }

    private async Task event_sold_out_integration_event_is_published()
    {
        var tickets = await getTicketsForEventEndpoint.GetTickets(event_id);
        tickets.Count.ShouldBe(17);
        tickets.All(t => t.Purchased).ShouldBeTrue();

        (await testHarness.Published.Any<EventSoldOut>(x => x.Context.Message.EventId == event_id)).ShouldBeTrue();
    }
}