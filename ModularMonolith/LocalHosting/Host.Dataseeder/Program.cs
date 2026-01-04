using System.Net.Http.Headers;
using Dataseeder.Hosting;
using Dataseeder.Infrastructure;
using Dataseeder.Seeders;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dataseeder;

public static class Program
{
    public static async Task Main()
    {
        var configuration = Configuration.Build();
        var settings = new Settings(configuration);
        JsonSerialization.RegisterConverters(Converters.GetConverters);

        var serviceProvider = BuildServiceProvider(settings);

        await using var eventPublisher = serviceProvider.GetRequiredService<KeycloakEventPublisher>();
        
        var userSeeder = serviceProvider.GetRequiredService<UserSeeder>();
        var venueSeeder = serviceProvider.GetRequiredService<VenueSeeder>();
        var eventSeeder = serviceProvider.GetRequiredService<EventSeeder>();
        var apiClient = serviceProvider.GetRequiredService<TicketBuddyApiClient>();
        var keycloakApiClient = serviceProvider.GetRequiredService<KeycloakApiClient>();

        if (await keycloakApiClient.GetUsersCountAsync() <= 1) await userSeeder.SeedAsync();

        await apiClient.EnsureAuthenticatedAsync();

        if (await apiClient.GetVenuesCountAsync() == 0) await venueSeeder.SeedAsync();

        if (await apiClient.GetEventsCountAsync() == 0)
        {
            var venueIds = await venueSeeder.GetVenueIdsAsync();
            await eventSeeder.SeedAsync(venueIds);
        }
    }

    private static ServiceProvider BuildServiceProvider(Settings settings)
    {
        const string apiClientName = "ApiClient";
        
        return new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton(settings)
            .AddHttpClient(apiClientName, client =>
            {
                client.BaseAddress = settings.Api.BaseUrl;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .Services
            .AddSingleton<KeycloakApiClient>()
            .AddSingleton<KeycloakEventPublisher>()
            .AddSingleton<TicketBuddyApiClient>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(apiClientName);
                return new TicketBuddyApiClient(httpClient, settings);
            })
            .AddSingleton<UserSeeder>()
            .AddSingleton<VenueSeeder>()
            .AddSingleton<EventSeeder>()
            .BuildServiceProvider();
    }
}