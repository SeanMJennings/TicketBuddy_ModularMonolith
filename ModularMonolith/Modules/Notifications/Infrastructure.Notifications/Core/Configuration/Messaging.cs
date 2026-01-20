using MassTransit;
using Messages.Tickets;
using Messaging.Notifications;

namespace Infrastructure.Notifications.Core.Configuration;

public static class Messaging
{
    public static void AddNotificationsConsumers(this IBusRegistrationConfigurator x)
    {
        var notificationsMessagingAssembly = NotificationsMessaging.Assembly;
        x.AddConsumers(notificationsMessagingAssembly);
    }

    public static void ConfigureNotificationsMessaging(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ReceiveEndpoint("notifications-queue", e =>
        {
            e.Bind<TicketPurchased>();
        });
    }
}