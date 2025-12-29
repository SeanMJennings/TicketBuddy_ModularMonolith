using Application.Tickets.Ticket;
using Domain.Tickets.Ticket;

namespace Application.Tickets.Core;

public static class TicketDomainEventsToHandlersMap
{
    public static readonly Dictionary<Type, Type> Map = new()
    {
        { typeof(AllTicketsSold), typeof(AllTicketsSoldHandler) }
    };
}