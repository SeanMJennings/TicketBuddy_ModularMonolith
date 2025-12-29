using Domain.DomainEvents;
using Domain.Tickets.Ticket;
using Messages.Tickets;

namespace Application.Tickets.Ticket;

public class AllTicketsSoldHandler(IPublishMessages publish) : HandleDomainEvents<AllTicketsSold>
{
    protected override async Task Handle(AllTicketsSold message)
    {
        var integrationEvent = new EventSoldOut
        {
            EventId = message.EventId
        };
        
        await publish.Publish(integrationEvent);
    }
}

public static class TicketDomainEventsToHandlersMap
{
    public static readonly Dictionary<Type, Type> Map = new()
    {
        { typeof(AllTicketsSold), typeof(AllTicketsSoldHandler) }
    };
}

