namespace TicketBuddy.AppHost.Infrastructure;

public static class RabbitMqResourceExtensions
{
    public const string ResourceName = "Messaging";
    private const string VolumeName = "Ticketbuddy.Aspire.RabbitMQ";
    private const string DefaultUsername = "guest";
    private const string DefaultPassword = "guest";
    private const int TcpPort = 5672;
    private const int ManagementPort = 15672;

    public static (IResourceBuilder<RabbitMQServerResource> RabbitMq,
        IResourceBuilder<ParameterResource> UserParam,
        IResourceBuilder<ParameterResource> PasswordParam) AddRabbitMqMessaging(this IDistributedApplicationBuilder builder)
    {
        var userParam = builder.AddParameter("RabbitMQUsername", DefaultUsername, secret: true);
        var passwordParam = builder.AddParameter("RabbitMQPassword", DefaultPassword, secret: true);

        var rabbitmq = builder
            .AddRabbitMQ(ResourceName, userName: userParam, password: passwordParam)
            .WithImage("rabbitmq")
            .WithDataVolume(VolumeName)
            .WithEndpoint("tcp", endpoint =>
            {
                endpoint.Port = TcpPort;
                endpoint.TargetPort = TcpPort;
            })
            .WithHttpEndpoint(port: ManagementPort, targetPort: ManagementPort, name: "management")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithComposeProjectLabel();

        return (rabbitmq, userParam, passwordParam);
    }
}