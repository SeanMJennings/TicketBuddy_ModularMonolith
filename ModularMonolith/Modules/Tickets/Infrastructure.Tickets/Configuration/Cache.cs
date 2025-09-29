using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.Tickets.Configuration;

public static class Cache
{
    public static void ConfigureCache(this IServiceCollection services, string redisConnectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var configuration = ConfigurationOptions.Parse(redisConnectionString, true);
            configuration.ResolveDns = true;
            return ConnectionMultiplexer.Connect(configuration);
        });
    }
}