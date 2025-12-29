namespace Application;

public interface IPublishMessages
{
    public Task Publish<T>(T message) where T : class;
}