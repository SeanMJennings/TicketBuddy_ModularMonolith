using System.ComponentModel.DataAnnotations;

namespace Domain.Tickets.Entities;

public class Ticket(Guid Id, Guid eventId, decimal price, uint SeatNumber, Guid? UserId = null, DateTimeOffset? PurchasedAt = null) : Entity(Id)
{
    public Guid EventId { get; private set; } = eventId;
    public decimal Price { get; private set; } = price;
    public uint SeatNumber { get; private set; } = SeatNumber;
    public Guid? UserId { get; private set; } = UserId;
    public DateTimeOffset? PurchasedAt { get; private set; } = PurchasedAt;
    
    public void Purchase(Guid userId)
    {
        if (UserId is not null) throw new ValidationException("Tickets are not available");
        UserId = userId;
        PurchasedAt = DateTimeOffset.UtcNow;
    }
    
    public void UpdatePrice(decimal newPrice)
    {
        if (UserId is not null) return;
        Price = newPrice;
    }
}