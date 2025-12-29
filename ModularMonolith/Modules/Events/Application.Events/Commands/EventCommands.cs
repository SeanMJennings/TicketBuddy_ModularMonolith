using Domain.Events.Contracts;
using Domain.Events.Services;
using Domain.ValueObjects;

namespace Application.Events.Commands;

public class EventCommands(EventsValidator eventsValidator, IPersistEvents eventRepository, IEventsUnitOfWork unitOfWork)
{
    public async Task<Guid> CreateEvent(EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, Money price)
    {
        var eventId = Guid.NewGuid();
        EventsValidator.ValidateDate(startDate);
        var theEvent = new Domain.Events.Entities.Event(eventId, eventName, startDate, endDate, Venue.FirstDirectArenaLeeds, price);
        
        await eventsValidator.CheckIfVenueAlreadyBooked(theEvent);
        await eventRepository.Add(theEvent);
        await unitOfWork.Commit();
        return eventId;
    }
    
    public async Task UpdateEvent(Guid eventId, EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, Money price)
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