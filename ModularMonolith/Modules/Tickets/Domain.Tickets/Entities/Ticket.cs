using System.ComponentModel.DataAnnotations;

namespace Domain.Tickets.Entities;

internal class Ticket : Entity
{
    private Ticket(Guid Id, Guid eventId, decimal price, uint seatNumber) : base(Id)
    {
        EventId = eventId;
        Price = price;
        SeatNumber = seatNumber;
    }
    private Ticket() : base(Guid.Empty) { }
    internal static Ticket Create(Guid id, Guid eventId, decimal price, uint seatNumber)
    {
        return new Ticket(id, eventId, price, seatNumber);
    }
    
    internal Guid EventId { get; private set; }
    internal decimal Price { get; private set; }
    internal uint SeatNumber { get; private set; }
    internal Guid? UserId { get; private set; }
    internal DateTimeOffset? PurchasedAt { get; private set; }
    
    internal void Purchase(Guid userId)
    {
        if (UserId is not null) throw new ValidationException("Tickets are not available");
        UserId = userId;
        PurchasedAt = DateTimeOffset.UtcNow;
    }
    
    internal void UpdatePrice(decimal newPrice)
    {
        if (UserId is not null) return;
        Price = newPrice;
    }
}