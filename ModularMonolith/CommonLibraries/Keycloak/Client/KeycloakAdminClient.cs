using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Keycloak.Client;

public static class KeycloakAdminClient
{
    public static async Task<HttpClient> CreateKeycloakAdminClient(Uri baseUrl, string clientId, string userName, string password)
    {
        var keycloakHttpClient = new HttpClient
        {
            BaseAddress = baseUrl
        };

        var adminToken = await GetKeycloakAdminToken(keycloakHttpClient, clientId, userName, password);

        var keycloakApiAdminHttpClient = new HttpClient
        {
            BaseAddress = baseUrl
        };
        keycloakApiAdminHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        keycloakApiAdminHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return keycloakApiAdminHttpClient;
    }
    
    private static async Task<string> GetKeycloakAdminToken(HttpClient client, string clientId, string userName, string password)
    {
        var tokenRequest = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "username", userName },
            { "password", password },
            { "grant_type", "password" }
        };

        var response = await client.PostAsync("/realms/master/protocol/openid-connect/token", new FormUrlEncodedContent(tokenRequest));
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        return tokenResponse!["access_token"].ToString()!;
    }
}