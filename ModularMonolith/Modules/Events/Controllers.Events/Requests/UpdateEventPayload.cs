using Domain.ValueObjects;

namespace Controllers.Events.Requests;

public record UpdateEventPayload(EventName EventName, DateTimeOffset StartDate, DateTimeOffset EndDate, decimal Price);