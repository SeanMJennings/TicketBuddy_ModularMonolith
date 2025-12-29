using Domain.DomainEvents;
using Domain.Tickets.DomainEvents;
using Messages.Tickets;

namespace Application.Tickets.DomainEventHandlers;

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
