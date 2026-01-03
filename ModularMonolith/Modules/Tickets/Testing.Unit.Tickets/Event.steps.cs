using BDD;
using Domain.Tickets.Event;
using Domain.ValueObjects;
using Shouldly;

namespace Unit;

public partial class EventSpecs : Specification
{
    private Guid id;
    private string name = null!;
    private DateTimeOffset start_date;
    private DateTimeOffset end_date;
    private Guid venueId;
    private Money price;
    private Event theEvent = null!;
    private Money updatedPrice;
    
    private const string invalid_name = "Jackie Chan 123!";
    private const string valid_name = "Jackie Chan 123";
    private const string updated_name = "Updated Event Name";

    protected override void before_each()
    {
        base.before_each();
        id = Guid.NewGuid();
        name = null!;
        venueId = Guid.Parse("22222222-2222-2222-2222-222222222222");
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
    
    private void creating_an_event()
    {
        var eventName = new EventName(name);
        theEvent = Event.Create(id, eventName, start_date, end_date, venueId, price);
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
        theEvent.UpdateVenue(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }
    
    private void the_event_is_created()
    {
        theEvent.ShouldNotBeNull();
        theEvent.Id.ShouldBe(id);
        theEvent.EventName.ToString().ShouldBe(valid_name);
        theEvent.StartDate.ShouldBe(start_date);
        theEvent.EndDate.ShouldBe(end_date);
        theEvent.VenueId.ShouldBe(venueId);
        theEvent.Price.ShouldBe(price);
    }
    
    private void event_name_is_updated()
    {
        theEvent.EventName.ToString().ShouldBe(updated_name);
    }
    
    private void event_dates_are_updated()
    {
        theEvent.StartDate.ShouldBe(start_date);
        theEvent.EndDate.ShouldBe(end_date);
    }
    
    private void event_price_is_updated()
    {
        theEvent.Price.ShouldBe(updatedPrice);
    }
    
    private void event_venue_is_updated()
    {
        theEvent.VenueId.ShouldBe(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }
}
