using MassTransit;
using Messages.Events;
using Messaging.Tickets;
using Messaging.Tickets.Consumers;

namespace Infrastructure.Tickets.Configuration;

public static class Messaging
{
    public static void AddTicketsConsumers(this IBusRegistrationConfigurator x)
    {
        var ticketsIntegrationMessagingAssembly = TicketsMessaging.Assembly;
        x.AddConsumers(ticketsIntegrationMessagingAssembly);
        x.AddConsumer<UserRegisteredConsumer, UserRegisteredConsumerDefinition>();
    }
    
    public static void ConfigureTicketsMessaging(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ReceiveEndpoint("tickets-queue", e =>
        {
            e.Bind<EventUpserted>();
            e.Bind<VenueUpserted>();
        });
    }
}