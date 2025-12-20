using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.Tickets.DomainEvents;
using Domain.ValueObjects;

namespace Domain.Tickets.Entities;

public class Event : Entity, IAmAnAggregateRoot
{
    internal Event(Guid id, EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, Domain.ValueObjects.Venue venue, Money price) : base(id)
    {
        if (endDate < startDate) throw new ValidationException("End date cannot be before start date");
        EventName = eventName;
        StartDate = startDate;
        EndDate = endDate;
        Venue = venue;
        Price = price;
    }
    
    public Event(Guid id, EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, Venue venue, Money price) : base(id)
    {
        if (endDate < startDate) throw new ValidationException("End date cannot be before start date");
        EventName = eventName;
        StartDate = startDate;
        EndDate = endDate;
        Venue = venue.Id;
        TheVenue = venue;
        Price = price;
    }
    
    public EventName EventName { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public Money Price { get; private set; }
    public Domain.ValueObjects.Venue Venue { get; private set; }
    internal Venue? TheVenue { get; private set; }
    
    public void UpdateName(EventName eventName) => EventName = eventName;
    
    public void UpdateDates(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (startDate < DateTimeOffset.UtcNow || endDate < DateTimeOffset.UtcNow) throw new ValidationException("Event date cannot be in the past");
        if (endDate < startDate) throw new ValidationException("End date cannot be before start date");
        StartDate = startDate;
        EndDate = endDate;
    }
    
    public void UpdateVenue(Venue venue)
    {
        TheVenue = venue;
        Venue = TheVenue.Id;
    }
    
    public void UpdatePrice(Money price) => Price = price;
    
    public void MarkAsSoldOut() => AddDomainEvent(new AllTicketsSold(Id));
}