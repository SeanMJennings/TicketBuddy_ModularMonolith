using Application.Contracts;
using Domain.Tickets.Contracts;
using Domain.Tickets.DomainEvents;
using Domain.Tickets.Entities;

namespace Application.Tickets.DomainEventHandlers;

public class EventUpsertedDomainEventHandler(IAmATicketRepositoryForCommands commandTicketRepository, IAmAnEventRepository eventRepository) 
    : DomainEventHandler<EventUpserted>
{
    protected override async Task Handle(EventUpserted domainEvent)
    {
        var existingTickets = (await commandTicketRepository.GetTicketsForEvent(domainEvent.Id)).ToArray();
        if (existingTickets.Length != 0)
        {
            UpdateExistingTicketsThatAreNotPurchased(domainEvent, existingTickets);
            await commandTicketRepository.Commit();
            return;
        }
        await ReleaseNewTickets(domainEvent);
        await commandTicketRepository.Commit();
    }

    private async Task ReleaseNewTickets(EventUpserted domainEvent)
    {
        var venue = await eventRepository.GetVenue(domainEvent.Venue);
        var newTickets = new List<Ticket>();
        for (uint i = 0; i < venue.Capacity; i++)
        {
            var ticket = new Ticket(
                Guid.NewGuid(),
                domainEvent.Id,
                domainEvent.Price,
                i + 1); 
            newTickets.Add(ticket);
        }
        commandTicketRepository.AddTickets(newTickets);
    }

    private void UpdateExistingTicketsThatAreNotPurchased(EventUpserted domainEvent, Ticket[] existingTickets)
    {
        var existingTicketsNotPurchased = existingTickets.Where(t => t.UserId == null).ToList();
        foreach (var ticket in existingTicketsNotPurchased)
        {
            ticket.UpdatePrice(domainEvent.Price);
        }
        commandTicketRepository.UpdateTickets(existingTicketsNotPurchased);
    }
}