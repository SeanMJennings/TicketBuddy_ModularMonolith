using BDD;
using Domain.Tickets.Entities;
using Domain.ValueObjects;
using Shouldly;

namespace Unit;

public partial class TicketSpecs : Specification
{
    private Guid ticketId;
    private Guid eventId;
    private Money price;
    private uint seatNumber;
    private Guid userId;
    private Ticket theTicket = null!;

    protected override void before_each()
    {
        base.before_each();
        ticketId = Guid.NewGuid();
        eventId = Guid.NewGuid();
        price = 25m;
        seatNumber = 1;
        userId = Guid.NewGuid();
        theTicket = null!;
    }

    private void valid_ticket_inputs()
    {
        ticketId = Guid.NewGuid();
        eventId = Guid.NewGuid();
        price = 25m;
        seatNumber = 1;
    }

    private void a_valid_ticket()
    {
        valid_ticket_inputs();
        creating_a_ticket();
    }

    private void a_purchased_ticket()
    {
        a_valid_ticket();
        theTicket.Purchase(Guid.NewGuid());
        theTicket.ClearDomainEvents();
    }

    private void creating_a_ticket()
    {
        theTicket = Ticket.Create(ticketId, eventId, price, seatNumber);
    }

    private void purchasing_the_ticket()
    {
        theTicket.Purchase(userId);
    }

    private void updating_the_ticket_price()
    {
        theTicket.UpdatePrice(30m);
    }

    private void the_ticket_is_created()
    {
        theTicket.ShouldNotBeNull();
        theTicket.Id.ShouldBe(ticketId);
        theTicket.EventId.ShouldBe(eventId);
        theTicket.Price.ShouldBe(price);
        theTicket.SeatNumber.ShouldBe(seatNumber);
        theTicket.IsAvailable.ShouldBeTrue();
        theTicket.UserId.ShouldBeNull();
        theTicket.PurchasedAt.ShouldBeNull();
    }

    private void the_ticket_is_purchased()
    {
        theTicket.UserId.ShouldBe(userId);
        theTicket.PurchasedAt.ShouldNotBeNull();
        theTicket.IsAvailable.ShouldBeFalse();
    }

    private void the_ticket_price_is_updated()
    {
        theTicket.Price.ShouldBe((Money)30m);
    }

    private void the_ticket_price_is_not_updated()
    {
        theTicket.Price.ShouldBe(price);
    }
}

