using Testcontainers.RabbitMq;

namespace Testing.Containers;

public static class RabbitMq
{
    public static RabbitMqContainer CreateContainer()
    {
        return new RabbitMqBuilder()
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(5673)
            .Build();
    }
}