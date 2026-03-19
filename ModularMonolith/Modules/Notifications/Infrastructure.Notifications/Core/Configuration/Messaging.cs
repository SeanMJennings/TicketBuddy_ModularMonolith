using MassTransit;
using Messages.Tickets;
using Messaging.Notifications.Consumers;

namespace Infrastructure.Notifications.Core.Configuration;

public static class Messaging
{
    public static void AddNotificationsConsumers(this IBusRegistrationConfigurator x)
    {
        x.AddConsumer<TicketPurchasedConsumer, TicketPurchasedConsumerDefinition>();
    }

    public static void AddNotificationsInbox(this IBusRegistrationConfigurator x)
    {
        x.AddEntityFrameworkOutbox<NotificationDbContext>(o =>
        {
            o.UsePostgres();
        });
    }

    public static void ConfigureNotificationsMessaging(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ReceiveEndpoint("notifications-queue", e =>
        {
            e.Bind<TicketPurchased>();
        });
    }
}

internal class TicketPurchasedConsumerDefinition : ConsumerDefinition<TicketPurchasedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<TicketPurchasedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<NotificationDbContext>(context);
    }
}