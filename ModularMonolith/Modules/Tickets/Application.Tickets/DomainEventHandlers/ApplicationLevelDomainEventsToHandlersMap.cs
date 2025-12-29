using Domain.Tickets.Ticket;

namespace Application.Tickets.DomainEventHandlers;

public static class ApplicationLevelDomainEventsToHandlersMap
{
    public static readonly Dictionary<Type, Type> Map = new()
    {
        { typeof(AllTicketsSold), typeof(AllTicketsSoldHandler) }
    };
}