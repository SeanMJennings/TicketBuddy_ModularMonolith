namespace Integration.Tickets.Messaging.Messages;

public record EventSoldOut
{
    public Guid EventId { get; init; }
}
