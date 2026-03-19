using System.ComponentModel.DataAnnotations;
using Domain.Exceptions;

namespace Domain.Events;

public static class EventsValidator
{
    public static Event CheckEventExists(Event? theEvent, Guid eventId) =>
        theEvent ?? throw new EntityNotFoundException(nameof(Event), eventId);

    public static void CheckIfVenueAlreadyBooked(Event theEvent, IList<Event> allEvents)
    {
        var conflictingEvent = allEvents.FirstOrDefault(e =>
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