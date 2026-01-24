using Common.Environment;
using NUnit.Framework;
using Testing.Containers;
using Testcontainers.PostgreSql;

namespace Integration;

[SetUpFixture]
public class Setup
{
    internal static PostgreSqlContainer Database { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task BeforeAll()
    {
        CommonEnvironment.LocalTesting.SetEnvironment();
        Database = PostgreSql.CreateContainer();
        await Database.StartAsync();
        Database.Migrate();
    }

    [OneTimeTearDown]
    public async Task AfterAll()
    {
        await Database.StopAsync();
        await Database.DisposeAsync();
        CommonEnvironment.LocalDevelopment.SetEnvironment();
    }
}