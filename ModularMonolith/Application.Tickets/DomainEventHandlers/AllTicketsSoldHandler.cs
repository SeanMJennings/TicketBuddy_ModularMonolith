using Application.Contracts;
using Domain.Tickets.DomainEvents;
using Integration.Tickets.Messaging.Messages;
using MassTransit;

namespace Application.Tickets.DomainEventHandlers;

public class AllTicketsSoldHandler(IPublishEndpoint publishEndpoint) : DomainEventHandler<AllTicketsSold>
{
    protected override async Task Handle(AllTicketsSold message)
    {
        var integrationEvent = new EventSoldOut
        {
            EventId = message.EventId
        };
        
        await publishEndpoint.Publish(integrationEvent);
    }
}
