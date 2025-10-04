using Application.Tickets;
using Integration.Users.Messaging.Messages;
using MassTransit;

namespace Infrastructure.Tickets.Configuration;

public static class Messaging
{
    public static void AddTicketsConsumers(this IBusRegistrationConfigurator x)
    {
        var ticketsIntegrationMessagingAssembly = TicketsIntegrationMessaging.Assembly;
        x.AddConsumers(ticketsIntegrationMessagingAssembly);
    }
    
    public static void ConfigureTicketsMessaging(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ReceiveEndpoint("tickets-queue", e =>
        {
            e.Bind<Integration.Events.Messaging.EventUpserted>();
        });
        cfg.ReceiveEndpoint("tickets-queue", e =>
        {
            e.Bind<UserUpserted>();
        });
    }
}