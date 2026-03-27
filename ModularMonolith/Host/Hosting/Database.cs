using Infrastructure.Events.Core.Configuration;
using Infrastructure.Messaging;
using Infrastructure.Notifications.Core.Configuration;
using Infrastructure.Tickets.Core.Configuration;

namespace Api.Hosting;

internal static class Database
{
    internal static void ConfigureDatabase(this IServiceCollection services, string connectionString)
    {
        services.ConfigureEventsDatabase(connectionString);
        services.ConfigureTicketsDatabase(connectionString);
        services.ConfigureNotificationsDatabase(connectionString);
        services.ConfigureSharedOutboxDatabase(connectionString);
    }
}