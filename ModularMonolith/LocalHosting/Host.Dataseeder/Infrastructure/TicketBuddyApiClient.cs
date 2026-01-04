using System.Net.Http.Headers;
using System.Net.Http.Json;
using Controllers.Events.Requests;
using Dataseeder.Hosting;
using Domain.Events;
using Domain.Events.Venue;
using Keycloak;
using EventRoutes = Controllers.Events.Routes;

namespace Dataseeder.Infrastructure;

internal class TicketBuddyApiClient(HttpClient httpClient, Settings settings)
{
    private bool _isAuthenticated;

    internal async Task EnsureAuthenticatedAsync()
    {
        if (_isAuthenticated) return;

        var keycloakJwt = await KeycloakClient.GetToken(
            settings.Keycloak.BaseUrl,
            settings.Keycloak.TicketBuddyRealm,
            settings.Keycloak.TicketBuddyApiClientId,
            settings.Keycloak.AdminUsername,
            settings.Keycloak.AdminPassword);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keycloakJwt);
        _isAuthenticated = true;
    }
    

    internal async Task<int> GetVenuesCountAsync()
    {
        var venues = await GetVenuesAsync();
        return venues.Length;
    }

    internal async Task<Venue[]> GetVenuesAsync()
    {
        var response = await httpClient.GetAsync(EventRoutes.Venues);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Venue[]>() ?? [];
    }

    internal async Task<int> GetEventsCountAsync()
    {
        var response = await httpClient.GetAsync(EventRoutes.Events);
        response.EnsureSuccessStatusCode();
        var events = await response.Content.ReadFromJsonAsync<Event[]>();
        return events!.Length;
    }

    internal async Task CreateVenueAsync(VenuePayload payload)
    {
        var response = await httpClient.PostAsJsonAsync(EventRoutes.Venues, payload);
        response.EnsureSuccessStatusCode();
        await response.Content.ReadFromJsonAsync<Guid>();
    }

    internal async Task CreateEventAsync(EventPayload payload)
    {
        var response = await httpClient.PostAsJsonAsync(
            EventRoutes.Events, 
            payload,
            JsonSerialization.GetJsonSerializerOptions());
        response.EnsureSuccessStatusCode();
    }
}
