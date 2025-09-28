using System.ComponentModel.DataAnnotations;
using EventName = Domain.Tickets.Primitives.EventName;

namespace Domain.Tickets.Entities;

public class Event : Entity, IAmAnAggregateRoot
{
    public Event(Guid id, EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, Venue venue, decimal price) : base(id)
    {
        if (endDate < startDate) throw new ValidationException("End date cannot be before start date");
        EventName = eventName;
        StartDate = startDate;
        EndDate = endDate;
        TheVenue = venue;
        Venue = TheVenue.Id;
        Price = price;
    }
    
    private Event() : base(Guid.Empty) { }
    
    public EventName EventName { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public decimal Price { get; private set; }
    public Domain.Events.Primitives.Venue Venue { get; private set; }
    internal List<Ticket> Tickets { get; private set; } = [];
    internal Venue TheVenue { get; private set; }
    
    public static Event Create(Guid id, EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, Venue venue, decimal price)
    {
        return new Event(id, eventName, startDate, endDate, venue, price);
    }
    
    public void UpdateName(EventName eventName) => EventName = eventName;
    public void UpdateDates(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (startDate < DateTimeOffset.UtcNow || endDate < DateTimeOffset.UtcNow) throw new ValidationException("Event date cannot be in the past");
        if (endDate < startDate) throw new ValidationException("End date cannot be before start date");
        StartDate = startDate;
        EndDate = endDate;
    }
    public void UpdateVenue(Venue venue) => TheVenue = venue;
    public void UpdatePrice(decimal price) => Price = price;
    
    public void UpdateExistingTicketsThatAreNotPurchased()
    {
        if (Tickets.Count == 0) throw new ValidationException("No tickets have been released for this event");
        
        var existingTicketsNotPurchased = Tickets.Where(t => t.UserId == null).ToList();
        foreach (var ticket in existingTicketsNotPurchased)
        {
            ticket.UpdatePrice(Price);
        }
    }

    public void ReleaseNewTickets()
    {
        if (Tickets.Count != 0) throw new ValidationException("Tickets have already been released for this event");
        
        for (uint i = 0; i < TheVenue.Capacity; i++)
        {
            var ticket = new Ticket(
                Guid.NewGuid(),
                Id,
                Price,
                i + 1);
            Tickets.Add(ticket);
        }
    }
    
    public void PurchaseTickets(Guid userId, Guid[] ticketIds)
    {
        var theTickets = Tickets.Where(t => ticketIds.Contains(t.Id)).ToArray();
        if (theTickets.Length != ticketIds.Length) throw new ValidationException("One or more tickets do not exist");
        
        foreach (var ticket in theTickets)
        {
            ticket.Purchase(userId);
        }
    }
}