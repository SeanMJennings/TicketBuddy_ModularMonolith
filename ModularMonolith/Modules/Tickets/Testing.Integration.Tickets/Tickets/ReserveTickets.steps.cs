using System.Security.Claims;
using Controllers.Tickets.Requests;
using Controllers.Tickets.Ticket;
using Infrastructure.Configuration;
using Infrastructure.Tickets.Configuration;
using Infrastructure.Tickets.Core.Configuration;
using MassTransit;
using MassTransit.Testing;
using Messages.Events;
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

public partial class ReserveTicketsSpecs : TruncateDbSpecification
{
    private GetTicketsForEventEndpoint getTicketsForEventEndpoint = null!;
    private ReserveTicketsEndpoint reserveTicketsEndpoint = null!;
    private EventUpsertedConsumer eventUpsertedConsumer = null!;
    private VenueUpsertedConsumer venueUpsertedConsumer = null!;
    private UserRegisteredConsumer userRegisteredConsumer = null!;
    private ServiceProvider serviceProvider = null!;
    private StackExchange.Redis.IConnectionMultiplexer cache = null!;
    private ITestHarness testHarness = null!;

    private Guid event_id = Guid.CreateVersion7();
    private Guid user_id = Guid.CreateVersion7();
    private readonly Guid another_user_id = Guid.CreateVersion7();
    private const decimal price = 25.00m;
    private const string name = "wibble";
    private const string email = "john.smith@gmail.com";
    private readonly DateTime event_start_date = DateTime.Now.AddDays(1);
    private readonly DateTime event_end_date = DateTime.Now.AddDays(1).AddHours(2);
    private Guid[] ticket_ids = null!;

    protected override Task before_each()
    {
        ticket_ids = [];
        event_id = Guid.CreateVersion7();
        user_id = Guid.CreateVersion7();

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureCache(Setup.Redis.GetConnectionString())
            .ConfigureTicketsDatabase(Setup.Database.GetConnectionString())
            .AddMassTransitTestHarness(x =>
            {
                x.AddTicketsConsumers();
                x.AddTicketsOutbox();
            })
            .AddSingleton(new Dictionary<Type, Type>())
            .ConfigureTicketsServices()
            .AddScoped<GetTicketsForEventEndpoint>()
            .AddScoped<ReserveTicketsEndpoint>()
            .BuildServiceProvider();

        testHarness = serviceProvider.GetRequiredService<ITestHarness>();
        testHarness.Start().GetAwaiter().GetResult();
        getTicketsForEventEndpoint = serviceProvider.GetRequiredService<GetTicketsForEventEndpoint>();
        reserveTicketsEndpoint = serviceProvider.GetRequiredService<ReserveTicketsEndpoint>();
        AddUserClaimToControllerContext(user_id);
        cache = serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
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

    private async Task requesting_the_tickets()
    {
        var tickets = await getTicketsForEventEndpoint.GetTickets(event_id);
        ticket_ids = tickets.Select(t => t.Id).ToArray();
    }

    private async Task reserving_a_ticket()
    {
        AddUserClaimToControllerContext(user_id);
        var payload = new TicketReservationPayload(ticket_ids.Take(1).ToArray());
        await reserveTicketsEndpoint.FastReserveTicketsWithoutCheckingForExistence(event_id, payload);
    }

    private async Task the_user_extends_their_reservation()
    {
        await reserving_a_ticket();
    }

    private async Task another_user_reserving_a_ticket()
    {
        AddUserClaimToControllerContext(another_user_id);
        var payload = new TicketReservationPayload(ticket_ids.Take(1).ToArray());
        await reserveTicketsEndpoint.FastReserveTicketsWithoutCheckingForExistence(event_id, payload);
    }

    private async Task the_ticket_is_reserved()
    {
        var tickets = await getTicketsForEventEndpoint.GetTickets(event_id);
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
        ttl.Value.TotalMinutes.ShouldBeLessThanOrEqualTo(15);
        ttl.Value.TotalMinutes.ShouldBeGreaterThan(14);
        var keyValue = db.StringGet(reservationKey);
        keyValue.HasValue.ShouldBeTrue();
        keyValue.ToString().ShouldBe(user_id.ToString());
    }
}