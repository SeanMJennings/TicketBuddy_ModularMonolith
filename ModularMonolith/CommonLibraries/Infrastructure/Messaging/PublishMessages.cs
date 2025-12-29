using Application;
using MassTransit;

namespace Infrastructure.Messaging;

public class PublishMessages(IPublishEndpoint publishEndpoint) : IPublishMessages
{
    public Task Publish<T>(T message) where T : class
    {
        return publishEndpoint.Publish(message);
    }
}