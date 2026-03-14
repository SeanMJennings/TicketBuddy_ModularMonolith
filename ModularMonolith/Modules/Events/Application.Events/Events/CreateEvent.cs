using Domain.Events;
using Domain.ValueObjects;

namespace Application.Events;

public class CreateEvent(EventsValidator eventsValidator, IPersistEvents eventRepository, IEventsUnitOfWork unitOfWork)
{
    public async Task<Guid> Execute(EventName eventName, DateTimeOffset startDate, DateTimeOffset endDate, Guid venueId, Money price)
    {
        var eventId = Guid.CreateVersion7();
        EventsValidator.ValidateDate(startDate);
        var theEvent = new Event(eventId, eventName, startDate, endDate, venueId, price);

        await eventsValidator.CheckIfVenueAlreadyBooked(theEvent);
        await eventRepository.Add(theEvent);
        await unitOfWork.Commit();
        return eventId;
    }
}