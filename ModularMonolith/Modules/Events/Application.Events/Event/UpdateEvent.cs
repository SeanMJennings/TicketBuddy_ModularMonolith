using Domain.Events.Contracts;
using Domain.Events.Services;
using Domain.ValueObjects;

namespace Application.Events.Event;

public class UpdateEvent(EventsValidator eventsValidator, IPersistEvents eventRepository, IEventsUnitOfWork unitOfWork)
{
    public async Task Execute(Guid eventId, EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, Money price)
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

