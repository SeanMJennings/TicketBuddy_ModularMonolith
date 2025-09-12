using Domain.DomainEvents;

namespace Application.Contracts;

public interface IAmADomainEventHandler
{
    public Task Handle(IAmADomainEvent message);
}

public abstract class DomainEventHandler<T> : IAmADomainEventHandler where T : IAmADomainEvent
{
    public async Task Handle(IAmADomainEvent message)
    {
        if (message is not T concreteMessage) throw new ArgumentException("Message is not of the expected type", nameof(message));
        await Handle(concreteMessage);
    }
    protected abstract Task Handle(T message);
}