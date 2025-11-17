using Testcontainers.RabbitMq;

namespace Testing.Containers;

public static class RabbitMq
{
    public const string UserName = "guest";
    public const string Password = "guest";
    public static RabbitMqContainer CreateContainer(int port = 5673)
    {
        return new RabbitMqBuilder()
            .WithUsername(UserName)
            .WithPassword(Password)
            .WithPortBinding(port)
            .Build();
    }
}