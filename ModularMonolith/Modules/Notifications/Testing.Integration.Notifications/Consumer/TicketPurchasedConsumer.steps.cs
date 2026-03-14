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
using Testing;

namespace Integration.Consumer;

public partial class TicketPurchasedConsumerSpecs : TruncateDbSpecification
{
    private TicketPurchasedConsumer consumer = null!;
    private ServiceProvider serviceProvider = null!;

    private Guid userId = Guid.CreateVersion7();
    private Guid ticketId = Guid.CreateVersion7();
    private Guid eventId = Guid.CreateVersion7();
    private string eventName = "Summer Concert 2026";
    private TicketPurchased message = null!;

    protected override async Task before_each()
    {
        await base.before_each();
        userId = Guid.CreateVersion7();
        ticketId = Guid.CreateVersion7();
        eventId = Guid.CreateVersion7();

        serviceProvider = new ServiceCollection()
            .ConfigureInfrastructureServices()
            .ConfigureNotificationsServices()
            .ConfigureNotificationsDatabase(Setup.Database.GetConnectionString())
            .AddSingleton(new Dictionary<Type, Type>())
            .AddScoped<Messaging.Notifications.Consumers.TicketPurchasedConsumer>()
            .BuildServiceProvider();

        consumer = serviceProvider.GetRequiredService<Messaging.Notifications.Consumers.TicketPurchasedConsumer>();
    }

    protected override async Task after_each()
    {
        await Truncate(Setup.Database.GetConnectionString());
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
        notification.Type.ShouldBe(NotificationType.TicketPurchased);
        notification.Payload.ShouldContain(eventName);
    }
}