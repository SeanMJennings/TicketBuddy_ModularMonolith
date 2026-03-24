using System.Diagnostics.CodeAnalysis;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Testing.Containers;

[ExcludeFromCodeCoverage]
public static class SharedContainers
{
    private static readonly bool IsRunningInCi =
        Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true" ||
        Environment.GetEnvironmentVariable("CI") == "true";

    private static string ContainerLabelPerAssembly => containerLabel ??=
        AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetName().Name ?? "")
            .FirstOrDefault(name => name.StartsWith("Testing."))
        ?? "default";

    private static string? containerLabel;

    private static PostgreSqlContainer? postgreSqlContainer;
    private static RedisContainer? redisContainer;
    private static RabbitMqContainer? rabbitMqContainer;
    private static KeycloakContainer? keycloakContainer;

    private static readonly SemaphoreSlim PostgreSqlSemaphore = new(1, 1);
    private static readonly SemaphoreSlim RedisSemaphore = new(1, 1);
    private static readonly SemaphoreSlim RabbitMqSemaphore = new(1, 1);
    private static readonly SemaphoreSlim KeycloakSemaphore = new(1, 1);

    private static bool migrationCompleted;

    public static async Task<PostgreSqlContainer> GetPostgreSqlAsync()
    {
        if (postgreSqlContainer is not null) return postgreSqlContainer;

        await PostgreSqlSemaphore.WaitAsync();
        try
        {
            if (postgreSqlContainer is not null) return postgreSqlContainer;

            postgreSqlContainer = PostgreSql.CreateContainer(!IsRunningInCi, ContainerLabelPerAssembly);
            await postgreSqlContainer.StartAsync();

            if (migrationCompleted) return postgreSqlContainer;

            postgreSqlContainer.Migrate();
            migrationCompleted = true;

            return postgreSqlContainer;
        }
        finally
        {
            PostgreSqlSemaphore.Release();
        }
    }

    public static async Task<RedisContainer> GetRedisAsync()
    {
        if (redisContainer is not null) return redisContainer;

        await RedisSemaphore.WaitAsync();
        try
        {
            if (redisContainer is not null) return redisContainer;

            redisContainer = Redis.CreateContainer(!IsRunningInCi, ContainerLabelPerAssembly);
            await redisContainer.StartAsync();
            return redisContainer;
        }
        finally
        {
            RedisSemaphore.Release();
        }
    }

    public static async Task<RabbitMqContainer> GetRabbitMqAsync()
    {
        if (rabbitMqContainer is not null) return rabbitMqContainer;

        await RabbitMqSemaphore.WaitAsync();
        try
        {
            if (rabbitMqContainer is not null) return rabbitMqContainer;

            rabbitMqContainer = RabbitMq.CreateContainer(!IsRunningInCi, ContainerLabelPerAssembly);
            await rabbitMqContainer.StartAsync();
            return rabbitMqContainer;
        }
        finally
        {
            RabbitMqSemaphore.Release();
        }
    }

    public static async Task<KeycloakContainer> GetKeycloakAsync()
    {
        var rabbit = await GetRabbitMqAsync();
        var rabbitMqUrl = new Uri($"amqp://{rabbit.Hostname}:{rabbit.GetMappedPublicPort(5672)}/");

        if (keycloakContainer is not null) return keycloakContainer;

        await KeycloakSemaphore.WaitAsync();
        try
        {
            if (keycloakContainer is not null) return keycloakContainer;

            keycloakContainer = Keycloak.CreateContainer(rabbitMqUrl, !IsRunningInCi);
            await keycloakContainer.StartAsync();
            return keycloakContainer;
        }
        finally
        {
            KeycloakSemaphore.Release();
        }
    }
}