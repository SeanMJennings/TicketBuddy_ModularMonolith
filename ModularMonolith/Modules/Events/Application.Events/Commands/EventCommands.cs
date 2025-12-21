using Domain.Contracts;
using Domain.Events.Contracts;
using Domain.Events.Entities;
using Domain.Events.Services;
using Domain.ValueObjects;

namespace Application.Events.Commands;

public class EventCommands(EventsValidator eventsValidator, IPersistEvents eventRepository, IUnitOfWork unitOfWork)
{
    public async Task<Guid> CreateEvent(EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, decimal price)
    {
        var eventId = Guid.NewGuid();
        EventsValidator.ValidateDate(startDate);
        var theEvent = new Event(eventId, eventName, startDate, endDate, Venue.FirstDirectArenaLeeds, price);
        
        await eventsValidator.CheckIfVenueAlreadyBooked(theEvent);
        await eventRepository.Add(theEvent);
        await unitOfWork.Commit();
        return eventId;
    }
    
    public async Task UpdateEvent(Guid eventId, EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, decimal price)
    {
        var existingEvent = await eventsValidator.CheckEventExists(eventId);
        EventsValidator.ValidateDate(startDate);
        existingEvent.UpdateName(eventName);
        existingEvent.UpdateDates(startDate, endDate);
        existingEvent.UpdatePrice(price);
        
        await eventsValidator.CheckIfVenueAlreadyBooked(existingEvent);
        await eventRepository.Update(existingEvent);
        await unitOfWork.Commit();
    }
}