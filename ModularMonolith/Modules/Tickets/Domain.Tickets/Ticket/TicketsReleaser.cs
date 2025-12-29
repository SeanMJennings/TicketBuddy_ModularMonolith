using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Core;
using Domain.ValueObjects;

namespace Domain.Tickets.Ticket;

public static class TicketsReleaser
{
    public static async Task ReleaseTicketsForEvent(Guid eventId, Money price, uint venueCapacity, IPersistTickets ticketRepository, ITicketsUnitOfWork unitOfWork)
    {
        var existingCount = await ticketRepository.GetTotalCountByEventId(eventId);
        if (existingCount > 0) throw new ValidationException("Tickets have already been released for this event");

        var tickets = new List<Ticket>();
        for (uint i = 0; i < venueCapacity; i++)
        {
            var ticket = new Ticket(
                Guid.NewGuid(),
                eventId,
                price,
                i + 1);
            tickets.Add(ticket);
        }

        await ticketRepository.AddRange(tickets);
        await unitOfWork.Commit();
    }
}