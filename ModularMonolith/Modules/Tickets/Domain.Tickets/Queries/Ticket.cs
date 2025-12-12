using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Tickets.Queries;

public class Ticket(Guid id, Guid eventId, decimal price, int seatNumber, bool purchased)
{
    public Guid Id { get; private set; } = id;
    public Guid EventId { get; private set; } = eventId;
    public decimal Price { get; private set; } = price;
    public int SeatNumber { get; private set; } = seatNumber;
    public bool Purchased { get; private set; } = purchased;
    
    [NotMapped]
    public bool Reserved { get; private set; }
    public void MarkTicketAsReserved() => Reserved = true;
}