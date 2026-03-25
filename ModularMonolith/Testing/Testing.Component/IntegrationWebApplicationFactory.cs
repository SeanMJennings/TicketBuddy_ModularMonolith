using Infrastructure.Events.Core.Configuration;
using Infrastructure.Tickets.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Component;

public class IntegrationWebApplicationFactory<TProgram>(string connectionString, string? redisConnectionString = null, string? rabbitMqConnectionString = null)
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        OverrideConfigurationThroughEnvironmentVariables();

        builder.ConfigureTestServices(services =>
        {
            services.AddTestAuthentication();
            services.AddTransient<IStartupFilter, ThrowingEndpointStartupFilter>();
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

internal sealed class ThrowingEndpointStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
    {
        next(app);
        app.Map("/test/throw", throwApp =>
            throwApp.Run(_ => throw new InvalidOperationException("deliberate test exception")));
    };
}

public static class TestAuthExtensions
{
    private const string Scheme = "TestScheme";

    public static IServiceCollection AddTestAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Scheme;
                options.DefaultChallengeScheme = Scheme;
            })
            .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>(Scheme, _ => { });

        services.AddAuthorization();
        return services;
    }
}