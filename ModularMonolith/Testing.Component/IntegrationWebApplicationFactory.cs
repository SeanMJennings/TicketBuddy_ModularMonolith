using Api.Hosting;
using Infrastructure.Events.Configuration;
using Infrastructure.Tickets.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Component;

public class IntegrationWebApplicationFactory<TProgram>(string connectionString, string? redisConnectionString = null)
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            if (redisConnectionString is not null) services.ConfigureCache(redisConnectionString);
            services.ConfigureDatabase(connectionString);
            services.ConfigureServices();
            services.AddMassTransitTestHarness(x =>
            {
                x.AddEventsConsumers();
                x.AddTicketsConsumers();
            });
        });

        builder.UseEnvironment("Test");
    }
}