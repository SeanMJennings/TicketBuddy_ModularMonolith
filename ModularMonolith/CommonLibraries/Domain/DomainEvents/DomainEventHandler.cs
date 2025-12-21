namespace Domain.DomainEvents;

public interface IHandleDomainEvents
{
    public Task Handle(IDescribeADomainEvent message);
}

public abstract class HandleDomainEvents<T> : IHandleDomainEvents where T : IDescribeADomainEvent
{
    public async Task Handle(IDescribeADomainEvent message)
    {
        if (message is not T concreteMessage) throw new ArgumentException("Message is not of the expected type", nameof(message));
        await Handle(concreteMessage);
    }
    protected abstract Task Handle(T message);
}