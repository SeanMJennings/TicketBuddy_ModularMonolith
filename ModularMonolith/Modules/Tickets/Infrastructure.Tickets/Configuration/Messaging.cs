using Application.Tickets;
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
            e.Bind<Integration.Users.Messaging.UserUpserted>();
        });
    }
}