using Infrastructure.Events.Configuration;
using Infrastructure.Tickets.Configuration;
using MassTransit;

namespace Api.Hosting;

internal static class Messaging
{
    internal static void ConfigureMessaging(this IServiceCollection services, Settings settings)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddEventsConsumers();
            x.AddTicketsConsumers();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(settings.RabbitMq.ConnectionString);
                cfg.ConfigureEventsMessaging();
                cfg.ConfigureTicketsMessaging();
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}