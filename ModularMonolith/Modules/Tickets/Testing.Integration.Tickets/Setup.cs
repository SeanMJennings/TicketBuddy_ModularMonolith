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
        Database = await SharedContainers.GetPostgreSqlAsync();
        Redis = await SharedContainers.GetRedisAsync();
    }

    [OneTimeTearDown]
    public Task AfterAll()
    {
        CommonEnvironment.LocalDevelopment.SetEnvironment();
        return Task.CompletedTask;
    }
}