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
        Database = await SharedContainers.GetPostgreSqlAsync();
    }

    [OneTimeTearDown]
    public Task AfterAll()
    {
        CommonEnvironment.LocalDevelopment.SetEnvironment();
        return Task.CompletedTask;
    }
}