using BDD;
using Domain.Events;
using Domain.ValueObjects;
using Shouldly;

namespace Unit;

public partial class EventSpecs : Specification
{
    private Guid id;
    private Guid venueId;
    private string name = null!;
    private DateTimeOffset start_date = DateTimeOffset.UtcNow.AddDays(1);
    private DateTimeOffset end_date = DateTimeOffset.UtcNow.AddDays(1).AddHours(2);
    private Event user = null!;
    private decimal price;

    private const string invalid_name = "Jackie Chan 123!";
    private const string valid_name = "Jackie Chan 123";

    protected override void before_each()
    {
        base.before_each();
        id = Guid.CreateVersion7();
        venueId = Guid.CreateVersion7();
        name = null!;
        user = null!;
        start_date = DateTimeOffset.UtcNow.AddDays(1);
        end_date = DateTimeOffset.UtcNow.AddDays(1).AddHours(2);
        price = 10m;
    }

    private void valid_inputs()
    {
        name = valid_name;
        start_date = DateTimeOffset.UtcNow.AddDays(1);
    }

    private void a_null_user_name()
    {
        name = null!;
    }    
    
    private void an_event_name()
    {
        name = string.Empty;
    }
    
    private void an_event_with_non_alphanumerical_characters()
    {
        name = invalid_name;
    }
    
    private void an_event_with_end_date_before_start_date()
    {
        start_date = DateTimeOffset.UtcNow.AddDays(2);
        end_date = DateTimeOffset.UtcNow.AddDays(1);
    }    
    
    private void an_event_with_negative_price()
    {
        price = -10m;
    }
    
    private void creating_an_event()
    {
        user = new Event(id, name, start_date, end_date, venueId, price);
    }

    private void the_event_is_created()
    {
        user.Id.ShouldBe(id);
        user.EventName.ToString().ShouldBe(valid_name);
        user.StartDate.ShouldBe(start_date);
        user.EndDate.ShouldBe(end_date);
        user.VenueId.ShouldBe(venueId);
        user.Price.ShouldBe(new Money(10m));
    }

    private void an_existing_event()
    {
        valid_inputs();
        creating_an_event();
    }

    private void updating_dates_with_start_date_in_the_past()
    {
        user.UpdateDates(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(2));
    }

    private void updating_dates_with_end_date_in_the_past()
    {
        user.UpdateDates(DateTimeOffset.UtcNow.AddDays(1), DateTimeOffset.UtcNow.AddDays(-1));
    }

    private void updating_dates_with_end_date_before_start_date()
    {
        user.UpdateDates(DateTimeOffset.UtcNow.AddDays(5), DateTimeOffset.UtcNow.AddDays(3));
    }

    private DateTimeOffset new_start_date;
    private DateTimeOffset new_end_date;

    private void updating_dates_with_valid_dates()
    {
        new_start_date = DateTimeOffset.UtcNow.AddDays(10);
        new_end_date = DateTimeOffset.UtcNow.AddDays(12);
        user.UpdateDates(new_start_date, new_end_date);
    }

    private void the_event_dates_are_updated()
    {
        user.StartDate.ShouldBe(new_start_date);
        user.EndDate.ShouldBe(new_end_date);
    }
}