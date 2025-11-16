using System.Net.Http.Headers;
using System.Net.Http.Json;
using Controllers.Events.Requests;
using Dataseeder.Hosting;
using Domain.Primitives;
using Keycloak;
using Keycloak.Client;
using Keycloak.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EventRoutes = Controllers.Events.Routes;

namespace Dataseeder;

public static class Program
{
    public static async Task Main()
    {
        var configuration = Configuration.Build();
        var settings = new Settings(configuration);

        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = settings.Api.BaseUrl;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .Services
            .BuildServiceProvider();

        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var apiHttpClient = httpClientFactory.CreateClient("ApiClient");
        var keycloakApiHttpClient = await KeycloakAdminClient.CreateKeycloakAdminClient(
            settings.Keycloak.BaseUrl,
            settings.Keycloak.ClientId,
            settings.Keycloak.AdminUsername,
            settings.Keycloak.AdminPassword);
        
        var count = await GetUsersCount(keycloakApiHttpClient);
        if (count > 1)
        {
            Console.WriteLine("Users already exist in Keycloak. Skipping data seeding.");
            return;
        }

        await CreateCustomerUsers(keycloakApiHttpClient);
        await CreateFutureEvents(apiHttpClient);
    }
    
    private static async Task<int> GetUsersCount(HttpClient client)
    {
        var response = await client.GetAsync("/admin/realms/ticketbuddy/users/count");
        response.EnsureSuccessStatusCode();
        var countResponse = await response.Content.ReadFromJsonAsync<int>();
        return countResponse!;
    }

    private static async Task CreateCustomerUsers(HttpClient client)
    {
        var customerData = new[]
        {
            (Name: "John Smith", Email: "john.smith@example.com"),
            (Name: "Jane Doe", Email: "jane.doe@example.com"),
            (Name: "Robert Johnson", Email: "robert.johnson@example.com"),
            (Name: "Emily Davis", Email: "emily.davis@example.com")
        };

        foreach (var customer in customerData)
        {
            var payload = new UserRepresentation
            {
                firstName = customer.Name.Split(' ')[0],
                lastName = customer.Name.Split(' ')[1],
                email = customer.Email,
                credentials =
                [
                    new CredentialRepresentation
                    {
                        value = customer.Name.Split(' ')[0].ToLowerInvariant() + customer.Name.Split(' ')[1].ToLowerInvariant()
                    }
                ]
            };
        
            var response = await client.PostAsJsonAsync("/admin/realms/ticketbuddy/users", payload);
            response.EnsureSuccessStatusCode();
        }
    }

    private static async Task CreateFutureEvents(HttpClient client)
    {
        var eventData = new[]
        {
            (Name: "Summer Rock Festival", StartDate: DateTime.Now.AddDays(30), EndDate: DateTime.Now.AddDays(30).AddHours(1), Venue: Venue.O2ArenaLondon, Price: 50m),
            (Name: "Classical Symphony", StartDate: DateTime.Now.AddDays(45), EndDate: DateTime.Now.AddDays(45).AddHours(1),Venue: Venue.RoyalAlbertHallLondon, Price: 75m),
            (Name: "International Football Match", StartDate: DateTime.Now.AddDays(60), EndDate: DateTime.Now.AddDays(60).AddHours(1),Venue: Venue.WembleyStadiumLondon, Price: 100m),
            (Name: "Comedy Night Special", StartDate: DateTime.Now.AddDays(15), EndDate: DateTime.Now.AddDays(15).AddHours(1),Venue: Venue.ManchesterArena, Price: 30m),
            (Name: "Tech Conference", StartDate: DateTime.Now.AddDays(90), EndDate: DateTime.Now.AddDays(90).AddHours(1), Venue: Venue.PrincipalityStadiumCardiff, Price: 200m),
            (Name: "Jazz Evening", StartDate: DateTime.Now.AddDays(20), EndDate: DateTime.Now.AddDays(20).AddHours(1), Venue: Venue.O2ArenaLondon, Price: 60m),
            (Name: "Pop Concert", StartDate: DateTime.Now.AddDays(25), EndDate: DateTime.Now.AddDays(25).AddHours(1), Venue: Venue.RoyalAlbertHallLondon, Price: 80m),
            (Name: "Basketball Championship", StartDate: DateTime.Now.AddDays(35), EndDate: DateTime.Now.AddDays(35).AddHours(1), Venue: Venue.WembleyStadiumLondon, Price: 120m),
            (Name: "Theater Play", StartDate: DateTime.Now.AddDays(40), EndDate: DateTime.Now.AddDays(40).AddHours(1), Venue: Venue.ManchesterArena, Price: 45m),
            (Name: "Business Summit", StartDate: DateTime.Now.AddDays(70), EndDate: DateTime.Now.AddDays(70).AddHours(1), Venue: Venue.PrincipalityStadiumCardiff, Price: 250m)
        };

        foreach (var eventInfo in eventData)
        {
            var payload = new EventPayload(
                eventInfo.Name,
                eventInfo.StartDate,
                eventInfo.EndDate,
                eventInfo.Venue,
                eventInfo.Price
            );

            var response = await client.PostAsJsonAsync(EventRoutes.Events, payload);
            response.EnsureSuccessStatusCode();
        }
    }
    

    
}
