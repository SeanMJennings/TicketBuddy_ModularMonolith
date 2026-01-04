using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Controllers.Events.Requests;
using Dataseeder.Hosting;
using Domain.Events;
using Keycloak;
using Keycloak.Requests;
using Messaging.Keycloak.Users;
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

            var venueIds = await CreateExampleVenues(apiHttpClient);
            await CreateFutureEvents(apiHttpClient, venueIds);
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
    
    private static async Task<Dictionary<string, Guid>> CreateExampleVenues(HttpClient client)
    {
        var venueData = new[]
        {
            (Key: "FirstDirectArena", Name: "First Direct Arena", Street: "Arena Way", City: "Leeds", Postcode: "LS2 8BY", Capacity: 50u),
            (Key: "OldTrafford", Name: "Old Trafford", Street: "Sir Matt Busby Way", City: "Manchester", Postcode: "M16 0RA", Capacity: 45u),
            (Key: "PrincipalityStadium", Name: "Principality Stadium", Street: "Westgate Street", City: "Cardiff", Postcode: "CF10 1NS", Capacity: 40u),
            (Key: "RoyalAlbertHall", Name: "Royal Albert Hall", Street: "Kensington Gore", City: "London", Postcode: "SW7 2AP", Capacity: 35u),
            (Key: "O2Arena", Name: "The O2 Arena", Street: "Peninsula Square", City: "London", Postcode: "SE10 0DX", Capacity: 50u)
        };

        var venueIds = new Dictionary<string, Guid>();

        foreach (var venue in venueData)
        {
            var payload = new VenuePayload(
                venue.Name,
                venue.Street,
                venue.City,
                venue.Postcode,
                venue.Capacity
            );

            var response = await client.PostAsJsonAsync(EventRoutes.Venues, payload);
            response.EnsureSuccessStatusCode();

            var venueId = await response.Content.ReadFromJsonAsync<Guid>();
            venueIds[venue.Key] = venueId;
        }

        return venueIds;
    }

    private static async Task RecreateKeycloakUserRegisteredEventFromKeycloak(Guid userId, string firstName, string lastName, string email)
    {
        var rabbitMqFactory = new ConnectionFactory
        {
            Uri = _settings.RabbitMq.ConnectionString
        };
        var rabbitMqConnection = await rabbitMqFactory.CreateConnectionAsync();
        await using var channel = await rabbitMqConnection.CreateChannelAsync();
        var details = new Dictionary<string, string>
        {
            { "first_name", firstName },
            { "last_name", lastName },
            { "email", email }
        };
        var message = JsonSerialization.Serialize(new UserRegistered(userId, details));
        var body = Encoding.UTF8.GetBytes(message);
        var properties = new BasicProperties
        {
            Persistent = true
        };
        await channel.BasicPublishAsync("amq.topic", "KK.EVENT.CLIENT.ticketbuddy.SUCCESS.ticketbuddy-ui.REGISTER", true, properties, body);
        await rabbitMqConnection.CloseAsync();
    }

    private static async Task CreateFutureEvents(HttpClient client, Dictionary<string, Guid> venueIds)
    {
        var eventData = new[]
        {
            (Name: "Summer Rock Festival", StartDate: DateTime.Now.AddDays(30), EndDate: DateTime.Now.AddDays(30).AddHours(1), VenueKey: "FirstDirectArena", Price: 50m),
            (Name: "Classical Symphony", StartDate: DateTime.Now.AddDays(45), EndDate: DateTime.Now.AddDays(45).AddHours(1), VenueKey: "RoyalAlbertHall", Price: 75m),
            (Name: "International Football Match", StartDate: DateTime.Now.AddDays(60), EndDate: DateTime.Now.AddDays(60).AddHours(1), VenueKey: "OldTrafford", Price: 100m),
            (Name: "Comedy Night Special", StartDate: DateTime.Now.AddDays(15), EndDate: DateTime.Now.AddDays(15).AddHours(1), VenueKey: "O2Arena", Price: 30m),
            (Name: "Tech Conference", StartDate: DateTime.Now.AddDays(90), EndDate: DateTime.Now.AddDays(90).AddHours(1), VenueKey: "PrincipalityStadium", Price: 200m),
            (Name: "Jazz Evening", StartDate: DateTime.Now.AddDays(20), EndDate: DateTime.Now.AddDays(20).AddHours(1), VenueKey: "FirstDirectArena", Price: 60m),
            (Name: "Pop Concert", StartDate: DateTime.Now.AddDays(25), EndDate: DateTime.Now.AddDays(25).AddHours(1), VenueKey: "RoyalAlbertHall", Price: 80m),
            (Name: "Basketball Championship", StartDate: DateTime.Now.AddDays(35), EndDate: DateTime.Now.AddDays(35).AddHours(1), VenueKey: "OldTrafford", Price: 120m),
            (Name: "Theater Play", StartDate: DateTime.Now.AddDays(40), EndDate: DateTime.Now.AddDays(40).AddHours(1), VenueKey: "O2Arena", Price: 45m),
            (Name: "Business Summit", StartDate: DateTime.Now.AddDays(70), EndDate: DateTime.Now.AddDays(70).AddHours(1), VenueKey: "PrincipalityStadium", Price: 250m)
        };

        foreach (var eventInfo in eventData)
        {
            var payload = new EventPayload(
                eventInfo.Name,
                eventInfo.StartDate,
                eventInfo.EndDate,
                venueIds[eventInfo.VenueKey],
                eventInfo.Price
            );

            var response = await client.PostAsJsonAsync(EventRoutes.Events, payload);
            response.EnsureSuccessStatusCode();
        }
    }
}