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
            And(a_null_user_name);
            When(Validating(creating_an_event));
            Then(Informs("EventName cannot be null or empty"));
        });        
        
        Scenario(() =>
        {
            Given(valid_inputs);
            And(an_event_name);
            When(Validating(creating_an_event));
            Then(Informs("EventName cannot be null or empty"));
        });
        
        Scenario(() =>
        {
            Given(valid_inputs);
            And(an_event_with_non_alphanumerical_characters);
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
    public void cannot_create_event_with_negative_price()
    {
        Given(valid_inputs);
        And(an_event_with_negative_price);
        When(Validating(creating_an_event));
        Then(Informs("Amount cannot be negative."));
    }
   
    [Test]
    public void can_create_valid_event()
    {
        Given(valid_inputs);
        When(creating_an_event);
        Then(the_event_is_created);
    }

    [Test]
    public void cannot_update_event_with_start_date_in_the_past()
    {
        Given(an_existing_event);
        When(Validating(updating_dates_with_start_date_in_the_past));
        Then(Informs("Event date cannot be in the past"));
    }

    [Test]
    public void cannot_update_event_with_end_date_in_the_past()
    {
        Given(an_existing_event);
        When(Validating(updating_dates_with_end_date_in_the_past));
        Then(Informs("Event date cannot be in the past"));
    }

    [Test]
    public void cannot_update_event_with_end_date_before_start_date()
    {
        Given(an_existing_event);
        When(Validating(updating_dates_with_end_date_before_start_date));
        Then(Informs("End date cannot be before start date"));
    }

    [Test]
    public void can_update_event_with_valid_dates()
    {
        Given(an_existing_event);
        When(updating_dates_with_valid_dates);
        Then(the_event_dates_are_updated);
    }
}