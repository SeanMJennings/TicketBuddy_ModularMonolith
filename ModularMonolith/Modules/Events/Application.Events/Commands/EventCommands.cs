using Domain.Events.Contracts;
using Domain.Events.Entities;
using Domain.Events.Services;
using Domain.Events.ValueObjects;
using Domain.Primitives;

namespace Application.Events.Commands;

public class EventCommands(EventsValidator eventsValidator, IPersistEvents eventRepository)
{
    public async Task<Guid> CreateEvent(EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, decimal price)
    {
        var eventId = Guid.NewGuid();
        Event.ValidateDate(startDate);
        var theEvent = new Event(eventId, eventName, startDate, endDate, Venue.FirstDirectArenaLeeds, price);
        
        await eventsValidator.CheckIfVenueAlreadyBooked(theEvent);
        await eventRepository.Add(theEvent);
        await eventRepository.Commit();
        return eventId;
    }
    
    public async Task UpdateEvent(Guid eventId, EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, decimal price)
    {
        var existingEvent = await eventsValidator.CheckEventExists(eventId);
        Event.ValidateDate(startDate);
        existingEvent.UpdateName(eventName);
        existingEvent.UpdateDates(startDate, endDate);
        existingEvent.UpdatePrice(price);
        
        await eventsValidator.CheckIfVenueAlreadyBooked(existingEvent);
        await eventRepository.Update(existingEvent);
        await eventRepository.Commit();
    }
}