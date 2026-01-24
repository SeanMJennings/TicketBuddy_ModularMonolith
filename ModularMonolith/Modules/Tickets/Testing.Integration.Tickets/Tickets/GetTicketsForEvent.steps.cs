using System.Security.Claims;
using Controllers.Tickets.Ticket;
using Infrastructure.Configuration;
using Infrastructure.Tickets.Configuration;
using Infrastructure.Tickets.Core.Configuration;
using MassTransit;
using MassTransit.Testing;
using Messages.Events;
using Messaging.Tickets.Consumers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Testing;
using Testing.Containers;

namespace Integration.Tickets;

public partial class GetTicketsForEventSpecs : TruncateDbSpecification
{
    private GetTicketsForEventEndpoint getTicketsForEventEndpoint = null!;
    private EventUpsertedConsumer eventUpsertedConsumer = null!;
    private VenueUpsertedConsumer venueUpsertedConsumer = null!;
    private ServiceProvider serviceProvider = null!;
    private ITestHarness testHarness = null!;

    private Guid event_id = Guid.NewGuid();
    private Guid user_id = Guid.NewGuid();
    private const decimal price = 25.00m;
    private const string name = "wibble";
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
            .BuildServiceProvider();

        testHarness = serviceProvider.GetRequiredService<ITestHarness>();
        testHarness.Start().GetAwaiter().GetResult();
        getTicketsForEventEndpoint = serviceProvider.GetRequiredService<GetTicketsForEventEndpoint>();
        AddUserClaimToControllerContext(user_id);
        eventUpsertedConsumer = serviceProvider.GetRequiredService<EventUpsertedConsumer>();
        venueUpsertedConsumer = serviceProvider.GetRequiredService<VenueUpsertedConsumer>();
        return Task.CompletedTask;
    }

    private void AddUserClaimToControllerContext(Guid userId)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", userId.ToString())], "TestAuth"));
        getTicketsForEventEndpoint.ControllerContext = new ControllerContext
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

    private async Task requesting_the_tickets()
    {
        var tickets = await getTicketsForEventEndpoint.GetTickets(event_id);
        ticket_ids = tickets.Select(t => t.Id).ToArray();
    }

    private async Task the_tickets_are_released()
    {
        var tickets = await getTicketsForEventEndpoint.GetTickets(event_id);
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
}