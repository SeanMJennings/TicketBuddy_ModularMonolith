using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Controllers.Events.Requests;
using Dataseeder.Hosting;
using Domain.Events.Entities;
using Domain.Primitives;
using Integration.Keycloak.Users.Messaging;
using Keycloak.Client;
using Keycloak.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using EventRoutes = Controllers.Events.Routes;

namespace Dataseeder;

public static class Program
{
    private static Settings _settings = null!;
    public static async Task Main()
    {
        var configuration = Configuration.Build();
        _settings = new Settings(configuration);

        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = _settings.Api.BaseUrl;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .Services
            .BuildServiceProvider();

        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var apiHttpClient = httpClientFactory.CreateClient("ApiClient");
        var keycloakApiHttpClient = await KeycloakClient.CreateKeycloakAdminClient(
            _settings.Keycloak.BaseUrl,
            _settings.Keycloak.MasterRealm,
            _settings.Keycloak.AdminCliClientId,
            _settings.Keycloak.AdminUsername,
            _settings.Keycloak.AdminPassword);
        
        if (await GetUsersCount(keycloakApiHttpClient) <= 1) await CreateCustomerUsers(keycloakApiHttpClient);
        
        if (await GetEventsCount(apiHttpClient) == 0)
        {
            var keycloakJwt = await KeycloakClient.GetToken(
                _settings.Keycloak.BaseUrl,
                _settings.Keycloak.TicketBuddyRealm,
                _settings.Keycloak.TicketBuddyApiClientId,
                _settings.Keycloak.AdminUsername,
                _settings.Keycloak.AdminPassword);
            apiHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keycloakJwt);
            await CreateFutureEvents(apiHttpClient);
        }
    }
    
    private static async Task<int> GetUsersCount(HttpClient client)
    {
        var response = await client.GetAsync("/admin/realms/ticketbuddy/users/count");
        response.EnsureSuccessStatusCode();
        var countResponse = await response.Content.ReadFromJsonAsync<int>();
        return countResponse!;
    }
    
    private static async Task<int> GetEventsCount(HttpClient client)
    {
        var response = await client.GetAsync(EventRoutes.Events);
        response.EnsureSuccessStatusCode();
        var events = await response.Content.ReadFromJsonAsync<Event[]>();
        return events!.Length;
    }

    private static async Task CreateCustomerUsers(HttpClient client)
    {
        var customerData = new[]
        {
            (UserId: Guid.NewGuid(), Name: "John Smith", Email: "john.smith@example.com"),
            (UserId: Guid.NewGuid(), Name: "Jane Doe", Email: "jane.doe@example.com"),
            (UserId: Guid.NewGuid(), Name: "Robert Johnson", Email: "robert.johnson@example.com"),
            (UserId: Guid.NewGuid(), Name: "Emily Davis", Email: "emily.davis@example.com")
        };

        foreach (var customer in customerData)
        {
            var payload = new UserRepresentation
            {
                id = customer.UserId,
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
            await RecreateKeycloakUserRegisteredEventFromKeycloak(customer.UserId, payload.firstName, payload.lastName, payload.email);
        }
    }
    
    private static async Task RecreateKeycloakUserRegisteredEventFromKeycloak(Guid user_id, string first_name, string last_name, string email)
    {
        var rabbitMqFactory = new ConnectionFactory
        {
            Uri = _settings.RabbitMq.ConnectionString
        };
        var rabbitMqConnection = await rabbitMqFactory.CreateConnectionAsync();
        await using var channel = await rabbitMqConnection.CreateChannelAsync();
        var details = new Dictionary<string, string>
        {
            { "first_name", first_name },
            { "last_name", last_name },
            { "email", email }
        };
        var message = JsonSerialization.Serialize(new UserRegistered(user_id, details));
        var body = Encoding.UTF8.GetBytes(message);
        var properties = new BasicProperties
        {
            Persistent = true
        };
        await channel.BasicPublishAsync("amq.topic", "KK.EVENT.CLIENT.ticketbuddy.SUCCESS.ticketbuddy-ui.REGISTER", true, properties, body);
        await rabbitMqConnection.CloseAsync();
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