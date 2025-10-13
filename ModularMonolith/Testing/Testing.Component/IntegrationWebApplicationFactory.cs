using Infrastructure.Events.Configuration;
using Infrastructure.Tickets.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Component;

public class IntegrationWebApplicationFactory<TProgram>(string connectionString, string? redisConnectionString = null, string? rabbitMqConnectionString = null)
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        OverrideConfigurationThroughEnvironmentVariables();

        builder.ConfigureTestServices(services =>
        {
            if (rabbitMqConnectionString is null)
            {
                services.AddMassTransitTestHarness(x =>
                {
                    x.AddEventsConsumers();
                    x.AddTicketsConsumers();
                });
            }
        });
    }

    private void OverrideConfigurationThroughEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__TicketBuddy", connectionString);
        if (redisConnectionString is not null)
            Environment.SetEnvironmentVariable("ConnectionStrings__Cache", redisConnectionString);
        if (rabbitMqConnectionString is not null)
            Environment.SetEnvironmentVariable("ConnectionStrings__Messaging", rabbitMqConnectionString);
    }
}