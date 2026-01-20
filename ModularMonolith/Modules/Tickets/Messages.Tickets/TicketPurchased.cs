namespace Messages.Tickets;

public record TicketPurchased
{
    public Guid UserId { get; init; }
    public Guid TicketId { get; init; }
    public Guid EventId { get; init; }
    public string EventName { get; init; } = null!;
}