using BDD;
using Domain.Tickets.Entities;
using Domain.Tickets.Primitives;
using FluentAssertions;

namespace Unit;

public partial class EventSpecs : Specification
{
    private Guid id;
    private string name = null!;
    private DateTimeOffset start_date;
    private DateTimeOffset end_date;
    private Venue venue = null!;
    private decimal price;
    private Event theEvent = null!;
    private Guid userId;
    private Guid[] ticketIds = [];
    private decimal updatedPrice;
    
    // Test constants
    private const string invalid_name = "Jackie Chan 123!";
    private const string valid_name = "Jackie Chan 123";
    private const string updated_name = "Updated Event Name";

    protected override void before_each()
    {
        base.before_each();
        id = Guid.NewGuid();
        userId = Guid.NewGuid();
        name = null!;
        venue = new Venue(Domain.Events.Primitives.Venue.EmiratesOldTraffordManchester, "Emirates Old Trafford, Manchester", 100);
        price = 25m;
        updatedPrice = 30m;
        start_date = DateTimeOffset.UtcNow.AddDays(1);
        end_date = DateTimeOffset.UtcNow.AddDays(1).AddHours(2);
        theEvent = null!;
    }
    
    private void valid_inputs()
    {
        name = valid_name;
        start_date = DateTimeOffset.UtcNow.AddDays(1);
        end_date = DateTimeOffset.UtcNow.AddDays(1).AddHours(2);
        price = 25m;
    }

    private void a_null_event_name()
    {
        name = null!;
    }    
    
    private void an_empty_event_name()
    {
        name = string.Empty;
    }
    
    private void an_event_name_with_non_alphanumerical_characters()
    {
        name = invalid_name;
    }
    
    private void an_event_with_end_date_before_start_date()
    {
        start_date = DateTimeOffset.UtcNow.AddDays(2);
        end_date = DateTimeOffset.UtcNow.AddDays(1);
    }
    
    private void valid_dates()
    {
        start_date = DateTimeOffset.UtcNow.AddDays(3);
        end_date = DateTimeOffset.UtcNow.AddDays(3).AddHours(2);
    }
    
    private void past_dates()
    {
        start_date = DateTimeOffset.UtcNow.AddDays(-1);
        end_date = DateTimeOffset.UtcNow.AddDays(-1).AddHours(2);
    }
    
    private void a_valid_event()
    {
        valid_inputs();
        creating_an_event();
    }
    
    private void tickets_already_released()
    {
        theEvent.ReleaseNewTickets();
        ticketIds = theEvent.Tickets.Select(t => t.Id).Take(2).ToArray();
    }
    
    private void updated_price()
    {
        price = updatedPrice;
    }
    
    private void nonexistent_ticket_ids()
    {
        ticketIds = [Guid.NewGuid(), Guid.NewGuid()];
    }
    
    private void creating_an_event()
    {
        var eventName = new EventName(name);
        theEvent = Event.Create(id, eventName, start_date, end_date, venue, price);
    }
    
    private void updating_event_name()
    {
        theEvent.UpdateName(new EventName(updated_name));
    }
    
    private void updating_event_dates()
    {
        theEvent.UpdateDates(start_date, end_date);
    }
    
    private void updating_event_price()
    {
        theEvent.UpdatePrice(updatedPrice);
    }
    
    private void updating_event_venue()
    {
        var newVenue = new Venue(Domain.Events.Primitives.Venue.FirstDirectArenaLeeds, "First Direct Arena, Leeds", 200);
        theEvent.UpdateVenue(newVenue);
    }
    
    private void releasing_tickets()
    {
        theEvent.ReleaseNewTickets();
    }
    
    private void updating_existing_tickets()
    {
        theEvent.UpdatePrice(updatedPrice);
        theEvent.UpdateExistingTicketsThatAreNotPurchased();
    }
    
    private void purchasing_tickets()
    {
        theEvent.PurchaseTickets(userId, ticketIds);
    }
    
    private void the_event_is_created()
    {
        theEvent.Should().NotBeNull();
        theEvent.Id.Should().Be(id);
        theEvent.EventName.ToString().Should().Be(valid_name);
        theEvent.StartDate.Should().Be(start_date);
        theEvent.EndDate.Should().Be(end_date);
        theEvent.TheVenue.Should().BeEquivalentTo(venue);
        theEvent.Price.Should().Be(price);
    }
    
    private void event_name_is_updated()
    {
        theEvent.EventName.ToString().Should().Be(updated_name);
    }
    
    private void event_dates_are_updated()
    {
        theEvent.StartDate.Should().Be(start_date);
        theEvent.EndDate.Should().Be(end_date);
    }
    
    private void event_price_is_updated()
    {
        theEvent.Price.Should().Be(updatedPrice);
    }
    
    private void event_venue_is_updated()
    {
        theEvent.TheVenue.Should().NotBeEquivalentTo(venue);
    }
    
    private void tickets_are_released()
    {
        theEvent.Tickets.Should().NotBeEmpty();
        theEvent.Tickets.Count.Should().Be(venue.Capacity);
        theEvent.Tickets.All(t => t.EventId == theEvent.Id).Should().BeTrue();
        theEvent.Tickets.All(t => t.Price == price).Should().BeTrue();
        theEvent.Tickets.All(t => t.UserId == null).Should().BeTrue();
    }
    
    private void ticket_prices_are_updated()
    {
        theEvent.Tickets.All(t => t.Price == updatedPrice).Should().BeTrue();
    }
    
    private void tickets_are_purchased()
    {
        foreach (var ticketId in ticketIds)
        {
            var ticket = theEvent.Tickets.FirstOrDefault(t => t.Id == ticketId);
            ticket.Should().NotBeNull();
            ticket.UserId.Should().Be(userId);
        }
    }
}
