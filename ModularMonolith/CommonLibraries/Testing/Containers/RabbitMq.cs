using System.Net.Http.Json;
using System.Text;
using Testcontainers.RabbitMq;

namespace Testing.Containers;

public static class RabbitMq
{
    public const string UserName = "guest";
    public const string Password = "guest";

    public static async Task Clear(this RabbitMqContainer container)
    {
        var managementPort = container.GetMappedPublicPort(15672);
        var baseUrl = $"http://{container.Hostname}:{managementPort}";

        using var httpClient = new HttpClient();
        var authBytes = Encoding.ASCII.GetBytes($"{UserName}:{Password}");
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

        var queuesResponse = await httpClient.GetAsync($"{baseUrl}/api/queues");
        if (!queuesResponse.IsSuccessStatusCode) return;

        var queues = await queuesResponse.Content.ReadFromJsonAsync<List<QueueInfo>>();
        if (queues is null) return;

        foreach (var queue in queues)
        {
            var encodedVhost = Uri.EscapeDataString(queue.vhost);
            var encodedName = Uri.EscapeDataString(queue.name);
            await httpClient.DeleteAsync($"{baseUrl}/api/queues/{encodedVhost}/{encodedName}/contents");
        }
    }

    private record QueueInfo(string name, string vhost);
}