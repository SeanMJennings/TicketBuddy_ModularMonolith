using MassTransit;
using Messaging.Events;

namespace Infrastructure.Events.Core.Configuration;

public static class Messaging
{
    public static void AddEventsConsumers(this IBusRegistrationConfigurator x)
    {
        var eventsIntegrationMessagingAssembly = EventsMessaging.Assembly;
        x.AddConsumers(eventsIntegrationMessagingAssembly);
    }

    public static void AddEventsOutbox(this IBusRegistrationConfigurator x)
    {
        x.AddEntityFrameworkOutbox<EventDbContext>(o =>
        {
            o.UsePostgres();
            o.UseBusOutbox();
            o.QueryDelay = TimeSpan.FromMilliseconds(100);
        });
    }

    public static void ConfigureEventsMessaging(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ReceiveEndpoint("events-queue", e =>
        {
            e.Bind<Messages.Tickets.EventSoldOut>();
        });
    }
}