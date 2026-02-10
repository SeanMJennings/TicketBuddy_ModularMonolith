using Migrations;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Testing.Containers;

public static class SharedContainers
{
    private const string LabelKey = "ticketbuddy-test";
    private const string PostgreSqlName = "ticketbuddy-test-postgresql";
    private const string RedisName = "ticketbuddy-test-redis";
    private const string RabbitMqName = "ticketbuddy-test-rabbitmq";
    private const string KeycloakName = "ticketbuddy-test-keycloak";

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

            postgreSqlContainer = new PostgreSqlBuilder("postgres:latest")
                .WithName(PostgreSqlName)
                .WithDatabase("TicketBuddy")
                .WithUsername("sa")
                .WithPassword("yourStrong(!)Password")
                .WithReuse(true)
                .WithLabel(LabelKey, "postgresql")
                .Build();

            await postgreSqlContainer.StartAsync();

            if (migrationCompleted) return postgreSqlContainer;

            Migration.Upgrade(postgreSqlContainer.GetConnectionString());
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

            redisContainer = new RedisBuilder("redis:latest")
                .WithName(RedisName)
                .WithReuse(true)
                .WithLabel(LabelKey, "redis")
                .Build();

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

            rabbitMqContainer = new RabbitMqBuilder("rabbitmq:management")
                .WithName(RabbitMqName)
                .WithUsername(RabbitMq.UserName)
                .WithPassword(RabbitMq.Password)
                .WithPortBinding(5672, true)
                .WithPortBinding(15672, true)
                .WithReuse(true)
                .WithLabel(LabelKey, "rabbitmq")
                .Build();

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

            var keycloakJarHostPath = Path.Combine(AppContext.BaseDirectory, "keycloak-to-rabbit-3.0.5.jar");

            keycloakContainer = new KeycloakBuilder("quay.io/keycloak/keycloak:26.3")
                .WithName(KeycloakName)
                .WithReuse(true)
                .WithLabel(LabelKey, "keycloak")
                .WithRealm("ticketbuddy-realm.json")
                .WithBindMount(keycloakJarHostPath, "/opt/keycloak/providers/keycloak-to-rabbit-3.0.5.jar")
                .WithUsername(Keycloak.AdminUserName)
                .WithPassword(Keycloak.AdminPassword)
                .WithEnvironment("KK_TO_RMQ_URL", rabbitMqUrl.ToString())
                .WithEnvironment("KK_TO_RMQ_VHOST", "/")
                .WithEnvironment("KK_TO_RMQ_USERNAME", RabbitMq.UserName)
                .WithEnvironment("KK_TO_RMQ_PASSWORD", RabbitMq.Password)
                .Build();

            await keycloakContainer.StartAsync();
            return keycloakContainer;
        }
        finally
        {
            KeycloakSemaphore.Release();
        }
    }
}