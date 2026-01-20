using Domain.Notifications;
using Infrastructure.Configuration;
using Infrastructure.Notifications.Core;
using Infrastructure.Notifications.Core.Configuration;
using MassTransit;
using Messages.Tickets;
using Messaging.Notifications.Consumers;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Testcontainers.PostgreSql;
using Testing;
using Testing.Containers;

namespace Integration;

public partial class TicketPurchasedConsumerSpecs : TruncateDbSpecification
{
    private TicketPurchasedConsumer consumer = null!;
    private ServiceProvider serviceProvider = null!;
    private static PostgreSqlContainer database = null!;

    private Guid userId = Guid.NewGuid();
    private Guid ticketId = Guid.NewGuid();
    private Guid eventId = Guid.NewGuid();
    private string eventName = "Summer Concert 2026";
    private TicketPurchased message = null!;

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        database.Migrate();
    }

    protected override async Task before_each()
    {
        await base.before_each();
        userId = Guid.NewGuid();
        ticketId = Guid.NewGuid();
        eventId = Guid.NewGuid();

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureNotificationsServices()
            .ConfigureNotificationsDatabase(database.GetConnectionString())
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<TicketPurchasedConsumer>()
            .BuildServiceProvider();

        consumer = serviceProvider.GetRequiredService<TicketPurchasedConsumer>();
    }

    protected override async Task after_each()
    {
        await Truncate(database.GetConnectionString());
    }

    protected override async Task after_all()
    {
        await database.StopAsync();
        await database.DisposeAsync();
    }

    private Task a_ticket_purchased_message()
    {
        message = new TicketPurchased
        {
            UserId = userId,
            TicketId = ticketId,
            EventId = eventId,
            EventName = eventName
        };
        return Task.CompletedTask;
    }

    private async Task the_consumer_processes_the_message()
    {
        var context = Substitute.For<ConsumeContext<TicketPurchased>>();
        context.Message.Returns(message);
        await consumer.Consume(context);
    }

    private async Task a_notification_is_created_for_the_user()
    {
        var repository = serviceProvider.GetRequiredService<IPersistNotifications>();
        var notifications = await repository.GetByUserId(userId);

        notifications.Count.ShouldBe(1);
        var notification = notifications[0];
        notification.UserId.ShouldBe(userId);
        ((string)notification.Type).ShouldBe("TicketPurchased");
        notification.Payload.ShouldContain(eventName);
    }
}