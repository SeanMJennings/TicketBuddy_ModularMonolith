using System.Net.Http.Json;
using Dataseeder.Hosting;
using Keycloak;
using Keycloak.Requests;

namespace Dataseeder.Infrastructure;

internal class KeycloakApiClient(Settings settings)
{
    private HttpClient? _httpClient;

    internal async Task<int> GetUsersCountAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync("/admin/realms/ticketbuddy/users/count");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    internal async Task CreateUserAsync(UserRepresentation payload)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/admin/realms/ticketbuddy/users", payload);
        response.EnsureSuccessStatusCode();
    }
    
    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        _httpClient ??= await KeycloakClient.CreateKeycloakAdminClient(
            settings.Keycloak.BaseUrl,
            settings.Keycloak.MasterRealm,
            settings.Keycloak.AdminCliClientId,
            settings.Keycloak.AdminUsername,
            settings.Keycloak.AdminPassword);

        return _httpClient;
    }
}

