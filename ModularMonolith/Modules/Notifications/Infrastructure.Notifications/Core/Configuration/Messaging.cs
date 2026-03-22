using MassTransit;
using Messages.Tickets;
using Messaging.Notifications;
using Messaging.Notifications.Consumers;

namespace Infrastructure.Notifications.Core.Configuration;

public static class Messaging
{
    public static void AddNotificationsConsumers(this IBusRegistrationConfigurator x)
    {
        var notificationIntegrationMessagingAssembly = NotificationsMessaging.Assembly;
        x.AddConsumers(notificationIntegrationMessagingAssembly);
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