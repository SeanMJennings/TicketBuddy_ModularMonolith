using Infrastructure.Events.Configuration;
using Infrastructure.Tickets.Configuration;

namespace Api.Hosting;

internal static class Database
{
    internal static void ConfigureDatabase(this IServiceCollection services, string connectionString)
    {
        services.ConfigureEventsDatabase(connectionString);
        services.ConfigureTicketsDatabase(connectionString);
    }
}