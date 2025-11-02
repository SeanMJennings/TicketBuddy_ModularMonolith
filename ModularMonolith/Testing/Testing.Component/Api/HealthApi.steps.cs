using System.Net;
using System.Text;
using BDD;
using Common.Environment;
using Controllers.Events;
using Controllers.Events.Requests;
using Domain.Primitives;
using Migrations;
using NUnit.Framework;
using Shouldly;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Testing;
using Testing.Containers;

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
    
    private const string application_json = "application/json";
    private const string name = "wibble";
    private readonly DateTimeOffset event_start_date = DateTimeOffset.UtcNow.AddDays(3);
    private readonly DateTimeOffset event_end_date = DateTimeOffset.UtcNow.AddDays(3).AddHours(2);
    private const decimal price = 12.34m;

    protected override async Task before_all()
    {
        CommonEnvironment.LocalDevelopment.SetEnvironment();
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        rabbit = RabbitMq.CreateContainer();
        await rabbit.StartAsync();
        redis = Redis.CreateContainer();
        await redis.StartAsync();
        database.Migrate();
    }
    
    protected override Task before_each()
    {
        base.before_each();
        factory = new IntegrationWebApplicationFactory<Program>(database.GetConnectionString(), redis.GetConnectionString(), rabbit.GetConnectionString());
        client = factory.CreateClient();
        response_content = null!;
        return Task.CompletedTask;
    }

    protected override async Task after_each()
    {
        database.Migrate();
        client.Dispose();
        await factory.DisposeAsync();
    }

    protected override async Task after_all()
    {
        await database.StopAsync();
        await database.DisposeAsync();
        await rabbit.StopAsync();
        await rabbit.DisposeAsync();
        await redis.StopAsync();
        await redis.DisposeAsync();
        CommonEnvironment.LocalTesting.SetEnvironment();
    }

    private void a_postgresql_database_is_available(){}
    private void a_redis_cache_is_available(){}
    private void a_rabbitmq_broker_is_available(){}

    private async Task the_api_is_running()
    {
        await ensureMassTransitIsAwakeAndWillPassHealthCheck();
    }

    private async Task ensureMassTransitIsAwakeAndWillPassHealthCheck()
    {
        var theContent = new StringContent(
            JsonSerialization.Serialize(new EventPayload(name, event_start_date, event_end_date, Venue.FirstDirectArenaLeeds, price)),
            Encoding.UTF8,
            application_json);
        var response = await client.PostAsync(Routes.Events, theContent);
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.Created);
        Thread.Sleep(1000);
    }

    private async Task calling_the_health_endpoint()
    {
        var response = await client.GetAsync("/health");
        response_code = response.StatusCode;
        response_content = response.Content;
    }
    
    private async Task the_response_is_ok()
    {
        var content = await response_content.ReadAsStringAsync();
        response_code.ShouldBe(HttpStatusCode.OK);
        content.ShouldContain("Healthy");
    }
}