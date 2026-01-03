using Domain.ValueObjects;

namespace Controllers.Events.Requests;

public record EventPayload(EventName EventName, DateTimeOffset StartDate, DateTimeOffset EndDate, Guid VenueId, Money Price);