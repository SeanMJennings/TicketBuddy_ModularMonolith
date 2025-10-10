using Api.Hosting;
using Infrastructure.Events.Configuration;
using Infrastructure.Tickets.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Acceptance;

public class IntegrationWebApplicationFactory<TProgram>(string connectionString, string redisConnectionString, string rabbitMqConnectionString)
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.ConfigureCache(redisConnectionString);
            services.ConfigureDatabase(connectionString);
            services.ConfigureServices();
            services.ConfigureMessaging(rabbitMqConnectionString);
        });

        builder.UseEnvironment("Test");
    }
}