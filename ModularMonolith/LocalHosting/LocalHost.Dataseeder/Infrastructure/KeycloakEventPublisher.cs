using System.Text;
using Dataseeder.Hosting;
using Messaging.Keycloak.Users;
using RabbitMQ.Client;

namespace Dataseeder.Infrastructure;

internal class KeycloakEventPublisher(Settings settings) : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    internal async Task PublishUserRegisteredAsync(Guid userId, string firstName, string lastName, string email)
    {
        await EnsureConnectionAsync();

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

        await _channel!.BasicPublishAsync(
            "amq.topic",
            "KK.EVENT.CLIENT.ticketbuddy.SUCCESS.ticketbuddy-ui.REGISTER",
            true,
            properties,
            body);
    }

    private async Task EnsureConnectionAsync()
    {
        if (_connection != null) return;

        var factory = new ConnectionFactory
        {
            Uri = settings.RabbitMq.ConnectionString
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
    }
}

