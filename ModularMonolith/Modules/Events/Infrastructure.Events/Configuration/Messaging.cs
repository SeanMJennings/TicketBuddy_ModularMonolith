using Application.Events;
using MassTransit;

namespace Infrastructure.Events.Configuration;

public static class Messaging
{
    public static void AddEventsConsumers(this IBusRegistrationConfigurator x)
    {
        var eventsIntegrationMessagingAssembly = EventsIntegrationMessaging.Assembly;
        x.AddConsumers(eventsIntegrationMessagingAssembly);
    }
    
    public static void ConfigureEventsMessaging(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ReceiveEndpoint("events-queue", e =>
        {
            e.Bind<Integration.Tickets.Messaging.Messages.EventSoldOut>();
        });
    }
}