using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Tickets.Ticket;

public class Ticket(Guid id, Guid eventId, Money price, uint seatNumber) : Entity(id), IAmAnAggregateRoot
{
    public Guid EventId { get; private set; } = eventId;
    public Money Price { get; private set; } = price;
    public uint SeatNumber { get; private set; } = seatNumber;
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