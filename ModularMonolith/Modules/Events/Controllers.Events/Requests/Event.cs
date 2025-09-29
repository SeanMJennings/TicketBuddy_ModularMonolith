using Domain.Events.Primitives;

namespace Controllers.Events.Requests;

public record EventPayload(EventName EventName, DateTimeOffset StartDate, DateTimeOffset EndDate, Venue Venue, decimal Price);
public record UpdateEventPayload(EventName EventName, DateTimeOffset StartDate, DateTimeOffset EndDate, decimal Price);