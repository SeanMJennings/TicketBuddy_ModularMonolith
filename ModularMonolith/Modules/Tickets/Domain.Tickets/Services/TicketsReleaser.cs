using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using Domain.ValueObjects;
using Venue = Domain.Tickets.Entities.Venue;

namespace Domain.Tickets.Services;

public static class TicketsReleaser
{
    public static async Task ReleaseTicketsForEvent(Guid eventId, Money price, Venue venue, IPersistTickets ticketRepository)
    {
        var existingCount = await ticketRepository.GetTotalCountByEventId(eventId);
        if (existingCount > 0) throw new ValidationException("Tickets have already been released for this event");

        var tickets = new List<Ticket>();
        for (var i = 0; i < venue.Capacity; i++)
        {
            var ticket = Ticket.Create(
                Guid.NewGuid(),
                eventId,
                price,
                (uint)(i + 1));
            tickets.Add(ticket);
        }

        await ticketRepository.SaveRange(tickets);
    }
}