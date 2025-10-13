using RabbitMQ.Client;

namespace Api.Hosting;

public static class Healthcheck
{
    public static void ConfigureHealthChecks(this IServiceCollection services, string postgresConnectionString, string redisConnectionString, string rabbitMqConnectionString)
    {
        services.AddSingleton<IConnection>(_ =>
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMqConnectionString),
            };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });
        services.AddHealthChecks()
            //.AddNpgSql(postgresConnectionString, name: "PostgreSQL", tags: ["ready"])
            //.AddRedis(redisConnectionString, name: "Redis", tags: ["ready"])
            .AddRabbitMQ();
    }
}