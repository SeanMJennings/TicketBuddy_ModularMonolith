namespace Messages.Tickets;

public record EventSoldOut
{
    public Guid EventId { get; init; }
}
