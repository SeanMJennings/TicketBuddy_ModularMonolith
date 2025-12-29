using Application.Tickets.Contracts;
using Domain.Tickets.Core;
using Domain.Tickets.Event;
using Domain.Tickets.Ticket;

namespace Application.Tickets.Commands;

public class TicketCommands(
    IPersistEvents eventRepository,
    IPersistTickets ticketRepository,
    ITicketsUnitOfWork unitOfWork,
    IPersistTicketReservationCache ticketReservationCache)
{
    public async Task PurchaseTickets(Guid eventId, Guid userId, Guid[] ticketIds)
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
            await eventRepository.Save(theEvent);
        }
        
        await unitOfWork.Commit();
    }

    public async Task ReserveTickets(Guid eventId, Guid userId, Guid[] ticketIds)
    {
        foreach (var ticketId in ticketIds)
        {
            await CheckIfTicketReservedForDifferentUser(eventId, ticketId, userId);
            await ticketReservationCache.ExtendTicketReservationForUser(eventId, ticketId, userId);
        }
    }
    
    private async Task CheckIfTicketReservedForDifferentUser(Guid eventId, Guid ticketId, Guid userId)
    {
        var userIdForReservation = await ticketReservationCache.GetUserIdForTicketReservation(eventId, ticketId);
        TicketsValidator.CheckIfTicketReservedForDifferentUser(userId, userIdForReservation);
    }
}