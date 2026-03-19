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
        var theEvent = TicketsValidator.CheckEventExists(await eventRepository.GetById(eventId), eventId);
        var theTickets = TicketsValidator.CheckTicketsExist(ticketIds, await ticketRepository.GetByIds(ticketIds));

        foreach (var ticketId in ticketIds) await EnsureTicketReservedForUser(eventId, ticketId, userId);

        var soldOut = await TicketsPurchaser.PurchaseTickets(eventId, userId, theTickets, ticketRepository);

        if (soldOut)
        {
            theEvent.MarkAsSoldOut();
            await eventRepository.Upsert(theEvent);
        }

        await unitOfWork.Commit();
    }

    private async Task EnsureTicketReservedForUser(Guid eventId, Guid ticketId, Guid userId)
    {
        var userIdForReservation = await ticketReservationCache.GetUserIdForTicketReservation(eventId, ticketId);
        TicketsValidator.EnsureTicketReservedForUser(userId, userIdForReservation);
    }
}