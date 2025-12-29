using Domain.Tickets.Event;

namespace Domain.Tickets.Core;

public static class DomainEventsToHandlersMap
{
    public static readonly Dictionary<Type, Type> Map = new()
    {
        { typeof(EventUpserted), typeof(EventUpsertedHandler) },
    };
}