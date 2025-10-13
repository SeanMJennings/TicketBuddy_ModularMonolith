using System.Net;
using BDD;
using Common.Environment;
using Migrations;
using NUnit.Framework;
using Shouldly;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Component.Api;

[TestFixture]
public partial class HealthApiSpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private HttpStatusCode response_code;
    private HttpContent response_content = null!;
    private static PostgreSqlContainer database = null!;
    private static RabbitMqContainer rabbit = null!;
    private static RedisContainer redis = null!;

    protected override void before_all()
    {
        CommonEnvironment.LocalDevelopment.SetEnvironment();
        database = new PostgreSqlBuilder()
            .WithDatabase("TicketBuddy")
            .WithUsername("sa")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1434)
            .Build();
        database.StartAsync().Await();
        rabbit = new RabbitMqBuilder()
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(5673)
            .Build();
        rabbit.StartAsync().Await();
        redis = new RedisBuilder()
            .WithPortBinding(6380)
            .Build();
        redis.StartAsync().Await();
        Migration.Upgrade(database.GetConnectionString());
    }
    
    protected override void before_each()
    {
        base.before_each();
        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString(), redis.GetConnectionString(), rabbit.GetConnectionString());
        client = factory.CreateClient();
        response_content = null!;
    }

    protected override void after_each()
    {
        Truncate(database.GetConnectionString());
        client.Dispose();
        factory.Dispose();
    }

    protected override void after_all()
    {
        database.StopAsync().Await();
        database.DisposeAsync().GetAwaiter().GetResult();
        rabbit.StopAsync().Await();
        rabbit.DisposeAsync().GetAwaiter().GetResult();
        redis.StopAsync().Await();
        redis.DisposeAsync().GetAwaiter().GetResult();
        CommonEnvironment.LocalTesting.SetEnvironment();
    }

    private void a_postgresql_database_is_available(){}
    private void a_redis_cache_is_available(){}
    private void a_rabbitmq_broker_is_available(){}
    private void the_api_is_running(){}
    
    private void calling_the_health_endpoint()
    {
        var response = client.GetAsync("/health").Await();
        response_code = response.StatusCode;
        response_content = response.Content;
    }
    
    private void the_response_is_ok()
    {
        var content = response_content.ReadAsStringAsync().Await();
        response_code.ShouldBe(HttpStatusCode.OK);
        content.ShouldContain("Healthy");
    }
}