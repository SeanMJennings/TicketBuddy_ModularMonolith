using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Tickets.Entities;

internal class Ticket : Entity
{
    private Ticket(Guid id, Guid eventId, Money price, uint seatNumber) : base(id)
    {
        EventId = eventId;
        Price = price;
        SeatNumber = seatNumber;
    }
    
    internal static Ticket Create(Guid id, Guid eventId, Money price, uint seatNumber)
    {
        return new Ticket(id, eventId, price, seatNumber);
    }
    
    internal Guid EventId { get; private set; }
    internal Money Price { get; private set; }
    internal uint SeatNumber { get; private set; }
    internal Guid? UserId { get; private set; }
    internal DateTimeOffset? PurchasedAt { get; private set; }
    
    internal void Purchase(Guid userId)
    {
        if (UserId is not null) throw new ValidationException("Tickets are not available");
        UserId = userId;
        PurchasedAt = DateTimeOffset.UtcNow;
    }
    
    internal void UpdatePrice(Money newPrice)
    {
        if (UserId is not null) return;
        Price = newPrice;
    }
}