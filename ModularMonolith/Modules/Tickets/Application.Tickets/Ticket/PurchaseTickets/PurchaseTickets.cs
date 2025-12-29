using Domain.Tickets.Core;
using Domain.Tickets.Event;
using Domain.Tickets.Ticket;

namespace Application.Tickets.Ticket.PurchaseTickets;

public class PurchaseTickets(
    IPersistEvents eventRepository,
    IPersistTickets ticketRepository,
    ITicketsUnitOfWork unitOfWork,
    IQueryTicketReservations ticketReservationCache)
{
    public async Task Execute(Guid eventId, Guid userId, Guid[] ticketIds)
    {
        foreach (var ticketId in ticketIds)
        {
            await CheckIfTicketReservedForDifferentUser(eventId, ticketId, userId);
        }
        
        var theEvent = await TicketsValidator.CheckEventExists(eventId, eventRepository);
        var soldOut = await TicketsPurchaser.PurchaseTickets(eventId, userId, ticketIds, ticketRepository);
        
        if (soldOut)
        {
            theEvent.MarkAsSoldOut();
            await eventRepository.Upsert(theEvent);
        }
        
        await unitOfWork.Commit();
    }
    
    private async Task CheckIfTicketReservedForDifferentUser(Guid eventId, Guid ticketId, Guid userId)
    {
        var userIdForReservation = await ticketReservationCache.GetUserIdForTicketReservation(eventId, ticketId);
        TicketsValidator.CheckIfTicketReservedForDifferentUser(userId, userIdForReservation);
    }
}