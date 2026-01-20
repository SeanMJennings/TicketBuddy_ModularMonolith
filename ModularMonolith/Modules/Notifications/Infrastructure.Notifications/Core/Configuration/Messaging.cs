using MassTransit;
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
    }
}