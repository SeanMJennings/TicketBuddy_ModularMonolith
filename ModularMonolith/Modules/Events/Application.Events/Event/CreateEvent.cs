using Domain.Events.Contracts;
using Domain.Events.Entities;
using Domain.Events.Services;
using Domain.ValueObjects;

namespace Application.Events.Event;

public class CreateEvent(EventsValidator eventsValidator, IPersistEvents eventRepository, IEventsUnitOfWork unitOfWork)
{
    public async Task<Guid> Execute(EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, Money price)
    {
        var eventId = Guid.NewGuid();
        EventsValidator.ValidateDate(startDate);
        var theEvent = new Domain.Events.Entities.Event(eventId, eventName, startDate, endDate, Venue.FirstDirectArenaLeeds, price);
        
        await eventsValidator.CheckIfVenueAlreadyBooked(theEvent);
        await eventRepository.Add(theEvent);
        await unitOfWork.Commit();
        return eventId;
    }
}

