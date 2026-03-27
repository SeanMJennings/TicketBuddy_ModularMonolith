using Infrastructure.Commands;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Messaging;

public static class OutboxConfiguration
{
    public static IServiceCollection ConfigureSharedOutboxDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<OutboxDbContext>(options =>
            options.UseNpgsql(connectionString, sqlOptions =>
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));
        services.AddScoped<IOutboxFlusher, OutboxFlusher>();
        return services;
    }

    public static void AddSharedOutbox(this IBusRegistrationConfigurator x)
    {
        x.AddEntityFrameworkOutbox<OutboxDbContext>(o =>
        {
            o.UsePostgres();
            o.UseBusOutbox();
            o.QueryDelay = TimeSpan.FromMilliseconds(100);
        });
    }
}