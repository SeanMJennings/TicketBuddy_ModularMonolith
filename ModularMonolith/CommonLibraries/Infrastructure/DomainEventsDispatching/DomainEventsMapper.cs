using Domain.DomainEvents;

namespace Infrastructure.DomainEventsDispatching;

public class DomainEventsMapper(Dictionary<Type, Type> map, IServiceProvider serviceProvider)
{
    public IHandleDomainEvents GetHandler(IDescribeADomainEvent domainEvent)
    {
        if (map.TryGetValue(domainEvent.GetType(), out var handlerType))
        {
            return serviceProvider.GetService(handlerType) as IHandleDomainEvents
                   ?? throw new InvalidOperationException($"Handler of type {handlerType} not found in the service provider.");
        }
        throw new KeyNotFoundException($"No handler found for domain event of type {domainEvent.GetType()}");
    }
}

public class DomainEventsMapBuilder
{
    private readonly Dictionary<Type, Type> _map = [];

    public DomainEventsMapBuilder Map<TEvent, THandler>()
        where TEvent : IDescribeADomainEvent
        where THandler : HandleDomainEvents<TEvent>
    {
        _map[typeof(TEvent)] = typeof(THandler);
        return this;
    }

    public Dictionary<Type, Type> Build() => _map;
}