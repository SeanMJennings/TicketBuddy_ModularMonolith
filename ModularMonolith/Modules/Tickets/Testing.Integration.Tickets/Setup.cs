using Common.Environment;
using NUnit.Framework;
using Testing.Containers;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Integration;

[SetUpFixture]
public class Setup
{
    internal static PostgreSqlContainer Database { get; private set; } = null!;
    internal static RedisContainer Redis { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task BeforeAll()
    {
        CommonEnvironment.LocalTesting.SetEnvironment();
        Database = PostgreSql.CreateContainer();
        await Database.StartAsync();
        Database.Migrate();
        Redis = Testing.Containers.Redis.CreateContainer();
        await Redis.StartAsync();
    }

    [OneTimeTearDown]
    public async Task AfterAll()
    {
        await Database.StopAsync();
        await Database.DisposeAsync();
        await Redis.StopAsync();
        await Redis.DisposeAsync();
        CommonEnvironment.LocalDevelopment.SetEnvironment();
    }
}