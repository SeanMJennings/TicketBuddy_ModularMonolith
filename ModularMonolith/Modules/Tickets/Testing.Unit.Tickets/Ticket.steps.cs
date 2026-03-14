using BDD;
using Domain.Tickets.Ticket;
using Domain.ValueObjects;
using Shouldly;

namespace Unit;

public partial class TicketQuerySpecs : Specification
{
    private Guid ticketId;
    private Guid eventId;
    private Money price;
    private uint seatNumber;
    private Guid userId;
    private Ticket _theTicketQuery = null!;

    protected override void before_each()
    {
        base.before_each();
        ticketId = Guid.CreateVersion7();
        eventId = Guid.CreateVersion7();
        price = 25m;
        seatNumber = 1;
        userId = Guid.CreateVersion7();
        _theTicketQuery = null!;
    }

    private void valid_ticket_inputs()
    {
        ticketId = Guid.CreateVersion7();
        eventId = Guid.CreateVersion7();
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
        _theTicketQuery.Purchase(Guid.CreateVersion7());
        _theTicketQuery.ClearDomainEvents();
    }

    private void creating_a_ticket()
    {
        _theTicketQuery = new Ticket(ticketId, eventId, price, seatNumber);
    }

    private void purchasing_the_ticket()
    {
        _theTicketQuery.Purchase(userId);
    }

    private void updating_the_ticket_price()
    {
        _theTicketQuery.UpdatePrice(30m);
    }

    private void the_ticket_is_created()
    {
        _theTicketQuery.ShouldNotBeNull();
        _theTicketQuery.Id.ShouldBe(ticketId);
        _theTicketQuery.EventId.ShouldBe(eventId);
        _theTicketQuery.Price.ShouldBe(price);
        _theTicketQuery.SeatNumber.ShouldBe(seatNumber);
        _theTicketQuery.IsAvailable.ShouldBeTrue();
        _theTicketQuery.UserId.ShouldBeNull();
        _theTicketQuery.PurchasedAt.ShouldBeNull();
    }

    private void the_ticket_is_purchased()
    {
        _theTicketQuery.UserId.ShouldBe(userId);
        _theTicketQuery.PurchasedAt.ShouldNotBeNull();
        _theTicketQuery.IsAvailable.ShouldBeFalse();
    }

    private void the_ticket_price_is_updated()
    {
        _theTicketQuery.Price.ShouldBe((Money)30m);
    }

    private void the_ticket_price_is_not_updated()
    {
        _theTicketQuery.Price.ShouldBe(price);
    }
}

