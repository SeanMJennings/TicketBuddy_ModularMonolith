using Infrastructure.Events.Core.Configuration;
using Infrastructure.Notifications.Core.Configuration;
using Infrastructure.Tickets.Configuration;
using MassTransit;

namespace Api.Hosting;

internal static class Messaging
{
    internal static void ConfigureMessaging(this IServiceCollection services, string rabbitMqConnectionString)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddEventsConsumers();
            x.AddTicketsConsumers();
            x.AddNotificationsConsumers();
            x.AddTicketsOutbox();
            x.AddEventsOutbox();
            x.AddNotificationsInbox();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqConnectionString);
                cfg.ConfigureEventsMessaging();
                cfg.ConfigureTicketsMessaging();
                cfg.ConfigureNotificationsMessaging();
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}