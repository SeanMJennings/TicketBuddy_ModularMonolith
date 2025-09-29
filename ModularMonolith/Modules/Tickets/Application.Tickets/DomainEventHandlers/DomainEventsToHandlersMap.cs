using Domain.Tickets.DomainEvents;

namespace Application.Tickets.DomainEventHandlers;

public static class DomainEventsToHandlersMap
{
    public static readonly Dictionary<Type, Type> Map = new()
    {
        { typeof(AllTicketsSold), typeof(AllTicketsSoldHandler) }
    };
}