using Domain.ValueObjects;

namespace Controllers.Events.Requests;

public record EventPayload(EventName EventName, DateTimeOffset StartDate, DateTimeOffset EndDate, Domain.ValueObjects.Venue Venue, decimal Price);