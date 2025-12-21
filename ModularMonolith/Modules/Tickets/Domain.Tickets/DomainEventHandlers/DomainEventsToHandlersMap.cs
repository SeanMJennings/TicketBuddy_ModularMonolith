using Domain.Tickets.DomainEvents;

namespace Domain.Tickets.DomainEventHandlers;

public static class DomainEventsToHandlersMap
{
    public static readonly Dictionary<Type, Type> Map = new()
    {
        { typeof(EventCreated), typeof(EventCreatedHandler) },
        { typeof(EventUpdated), typeof(EventUpdatedHandler) },
    };
}