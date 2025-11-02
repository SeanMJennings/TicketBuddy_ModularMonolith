using Testcontainers.RabbitMq;

namespace Testing.Containers;

public static class RabbitMq
{
    public static RabbitMqContainer CreateContainer(int port = 5673)
    {
        return new RabbitMqBuilder()
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(port)
            .Build();
    }
}