using Application.Contracts;
using Domain.DomainEvents;

namespace Infrastructure.DomainEventsDispatching;

public class DomainEventsMapper(Dictionary<Type, Type> Map, IServiceProvider ServiceProvider)
{
    public IHandleDomainEvents GetHandler(IDescribeADomainEvent domainEvent)
    {
        if (Map.TryGetValue(domainEvent.GetType(), out var handlerType))
        {
            return ServiceProvider.GetService(handlerType) is not IHandleDomainEvents handler 
                ? throw new InvalidOperationException($"Handler of type {handlerType} not found in the service provider.") : handler;
        }
        throw new KeyNotFoundException($"No handler found for domain event of type {domainEvent.GetType()}");
    }
}