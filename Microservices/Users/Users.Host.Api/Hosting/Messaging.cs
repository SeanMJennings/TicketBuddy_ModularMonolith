﻿using MassTransit;
using Users.Persistence;

namespace Api.Hosting;

internal static class Messaging
{
    internal static void ConfigureMessaging(this IServiceCollection services, Settings settings)
    {
        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<UserDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });
            x.UsingRabbitMq((_, cfg) =>
            {
                cfg.Host(settings.RabbitMq.ConnectionString);
            });
        });
    }
}