using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Tickets.Entities;

public class Ticket : Entity, IAmAnAggregateRoot
{
    public Ticket(Guid id, Guid eventId, Money price, uint seatNumber) : base(id)
    {
        EventId = eventId;
        Price = price;
        SeatNumber = seatNumber;
    }
    
    public Guid EventId { get; private set; }
    public Money Price { get; private set; }
    public uint SeatNumber { get; private set; }
    public Guid? UserId { get; private set; }
    public DateTimeOffset? PurchasedAt { get; private set; }
    
    public bool IsAvailable => UserId is null;
    
    public void Purchase(Guid userId)
    {
        if (UserId is not null) throw new ValidationException("Tickets are not available");
        UserId = userId;
        PurchasedAt = DateTimeOffset.UtcNow;
    }
    
    public void UpdatePrice(Money newPrice)
    {
        if (!IsAvailable) return;
        Price = newPrice;
    }
}