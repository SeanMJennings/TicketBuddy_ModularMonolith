using BDD;
using Domain.Events.Entities;
using Domain.ValueObjects;
using Shouldly;

namespace Unit;

public partial class EventSpecs : Specification
{
    private Guid id;
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
        id = Guid.NewGuid();
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
        user = new Event(id, name, start_date, end_date, Venue.FirstDirectArenaLeeds, price);
    }    
    
    private void the_event_is_created()
    {
        user.Id.ShouldBe(id);
        user.EventName.ToString().ShouldBe(valid_name);
        user.StartDate.ShouldBe(start_date);
        user.EndDate.ShouldBe(end_date);
        user.Venue.ShouldBe(Venue.FirstDirectArenaLeeds);
        user.Price.ShouldBe(new Money(10m));
    }
}