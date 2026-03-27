using MassTransit;
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
}