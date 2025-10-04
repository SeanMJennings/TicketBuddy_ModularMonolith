using System.ComponentModel.DataAnnotations;
using Domain.Events.Contracts;
using Domain.Events.Entities;
using Domain.Events.Primitives;
using Domain.Primitives;

namespace Application.Events.Commands;

public class EventCommands(IAmAnEventRepository EventRepository)
{
    public async Task<Guid> CreateEvent(EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, decimal price)
    {
        var eventId = Guid.NewGuid();
        ValidateDate(startDate);
        var theEvent = new Event(eventId, eventName, startDate, endDate, Venue.FirstDirectArenaLeeds, price);
        
        await CheckIfVenueAlreadyBooked(theEvent);
        await EventRepository.Add(theEvent);
        await EventRepository.Commit();
        return eventId;
    }
    
    public async Task UpdateEvent(Guid eventId, EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, decimal price)
    {
        var existingEvent = await CheckEventExists(eventId);
        ValidateDate(startDate);
        existingEvent.UpdateName(eventName);
        existingEvent.UpdateDates(startDate, endDate);
        existingEvent.UpdatePrice(price);
        
        await CheckIfVenueAlreadyBooked(existingEvent);
        await EventRepository.Update(existingEvent);
        await EventRepository.Commit();
    }

    private async Task<Event> CheckEventExists(Guid eventId)
    {
        var existingEvent = await EventRepository.Get(eventId);
        return existingEvent ?? throw new ValidationException($"Event with id {eventId} not found");
    }

    private async Task CheckIfVenueAlreadyBooked(Event theEvent)
    {
        var events = await EventRepository.GetAll();
        var conflictingEvent = events.FirstOrDefault(e =>
            e.Venue == theEvent.Venue &&
            e.Id != theEvent.Id &&
            ((theEvent.StartDate >= e.StartDate && theEvent.StartDate < e.EndDate) ||
             (theEvent.EndDate > e.StartDate && theEvent.EndDate <= e.EndDate) ||
             (theEvent.StartDate <= e.StartDate && theEvent.EndDate >= e.EndDate)));
        
        if (conflictingEvent is not null) throw new ValidationException("Venue is not available at the selected time");
    }
    
    private static void ValidateDate(DateTimeOffset startDate)
    {
        if (startDate < DateTimeOffset.UtcNow) throw new ValidationException("Event date cannot be in the past");
    }
}