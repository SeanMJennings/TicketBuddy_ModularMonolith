using System.ComponentModel.DataAnnotations;

namespace Domain.Events;

public class EventsValidator(IPersistEvents eventRepository)
{
    public async Task<Event> CheckEventExists(Guid eventId)
    {
        var existingEvent = await eventRepository.Get(eventId);
        return existingEvent ?? throw new ValidationException($"Event with id {eventId} not found");
    }

    public async Task CheckIfVenueAlreadyBooked(Event theEvent)
    {
        var events = await eventRepository.GetAll();
        var conflictingEvent = events.FirstOrDefault(e =>
            e.VenueId == theEvent.VenueId &&
            e.Id != theEvent.Id &&
            ((theEvent.StartDate >= e.StartDate && theEvent.StartDate < e.EndDate) ||
             (theEvent.EndDate > e.StartDate && theEvent.EndDate <= e.EndDate) ||
             (theEvent.StartDate <= e.StartDate && theEvent.EndDate >= e.EndDate)));

        if (conflictingEvent is not null) throw new ValidationException("Venue is not available at the selected time");
    }
    
    public static void ValidateDate(DateTimeOffset startDate)
    {
        if (startDate < DateTimeOffset.UtcNow) throw new ValidationException("Event date cannot be in the past");
    }
}