using NUnit.Framework;

namespace Unit;

public partial class EventSpecs
{
    [Test]
    public void an_event_must_have_a_name()
    {
        Scenario(() =>
        {
            Given(valid_inputs);
            And(a_null_event_name);
            When(Validating(creating_an_event));
            Then(Informs("Name cannot be empty"));
        });        
        
        Scenario(() =>
        {
            Given(valid_inputs);
            And(an_empty_event_name);
            When(Validating(creating_an_event));
            Then(Informs("Name cannot be empty"));
        });
        
        Scenario(() =>
        {
            Given(valid_inputs);
            And(an_event_name_with_non_alphanumerical_characters);
            When(Validating(creating_an_event));
            Then(Informs("Name can only have alphanumerical characters"));
        }); 
    }
    
    [Test]
    public void cannot_create_event_with_end_date_before_start_date()
    {
        Given(valid_inputs);
        And(an_event_with_end_date_before_start_date);
        When(Validating(creating_an_event));
        Then(Informs("End date cannot be before start date"));
    }
    
    [Test]
    public void can_create_valid_event()
    {
        Given(valid_inputs);
        When(creating_an_event);
        Then(the_event_is_created);
    }
    
    [Test]
    public void can_update_event_name()
    {
        Given(a_valid_event);
        When(updating_event_name);
        Then(event_name_is_updated);
    }
    
    [Test]
    public void can_update_event_dates()
    {
        Given(a_valid_event);
        And(valid_dates);
        When(updating_event_dates);
        Then(event_dates_are_updated);
    }
    
    [Test]
    public void cannot_update_event_dates_to_past_dates()
    {
        Given(a_valid_event);
        And(past_dates);
        When(Validating(updating_event_dates));
        Then(Informs("Event date cannot be in the past"));
    }
    
    [Test]
    public void cannot_update_event_with_end_date_before_start_date()
    {
        Given(a_valid_event);
        And(an_event_with_end_date_before_start_date);
        When(Validating(updating_event_dates));
        Then(Informs("End date cannot be before start date"));
    }
    
    [Test]
    public void can_update_event_price()
    {
        Given(a_valid_event);
        When(updating_event_price);
        Then(event_price_is_updated);
    }
    
    [Test]
    public void can_update_event_venue()
    {
        Given(a_valid_event);
        When(updating_event_venue);
        Then(event_venue_is_updated);
    }
    
    [Test]
    public void can_release_tickets()
    {
        Given(a_valid_event);
        When(releasing_tickets);
        Then(tickets_are_released);
    }
    
    [Test]
    public void cannot_release_tickets_when_tickets_already_exist()
    {
        Given(a_valid_event);
        And(tickets_already_released);
        When(Validating(releasing_tickets));
        Then(Informs("Tickets have already been released for this event"));
    }
    
    [Test]
    public void can_update_existing_tickets_price()
    {
        Given(a_valid_event);
        And(tickets_already_released);
        And(updated_price);
        When(updating_existing_tickets);
        Then(ticket_prices_are_updated);
    }
    
    [Test]
    public void cannot_update_tickets_when_no_tickets_exist()
    {
        Given(a_valid_event);
        When(Validating(updating_existing_tickets));
        Then(Informs("No tickets have been released for this event"));
    }
    
    [Test]
    public void can_purchase_tickets()
    {
        Given(a_valid_event);
        And(tickets_already_released);
        When(purchasing_tickets);
        Then(tickets_are_purchased);
    }
    
    [Test]
    public void cannot_purchase_tickets_that_do_not_exist()
    {
        Given(a_valid_event);
        And(tickets_already_released);
        And(nonexistent_ticket_ids);
        When(Validating(purchasing_tickets));
        Then(Informs("One or more tickets do not exist"));
    }
}
