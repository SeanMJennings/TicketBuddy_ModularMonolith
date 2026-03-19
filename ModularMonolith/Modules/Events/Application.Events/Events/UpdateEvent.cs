using Domain.Events;
using Domain.ValueObjects;

namespace Application.Events;

public class UpdateEvent(IPersistEvents eventRepository, IEventsUnitOfWork unitOfWork)
{
    public async Task Execute(Guid eventId, EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, Money price)
    {
        var existingEvent = EventsValidator.CheckEventExists(await eventRepository.Get(eventId), eventId);
        EventsValidator.ValidateDate(startDate);
        existingEvent.UpdateName(eventName);
        existingEvent.UpdateDates(startDate, endDate);
        existingEvent.UpdatePrice(price);

        var allEvents = await eventRepository.GetAll();
        EventsValidator.CheckIfVenueAlreadyBooked(existingEvent, allEvents);
        await eventRepository.Update(existingEvent);
        await unitOfWork.Commit();
    }
}