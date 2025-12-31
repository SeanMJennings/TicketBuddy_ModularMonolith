using NUnit.Framework;

namespace Unit;

public partial class VenueSpecs
{
    [Test]
    public void can_create_valid_venue()
    {
        Given(valid_inputs);
        When(creating_a_venue);
        Then(the_venue_is_created);
    }
    
    [Test]
    public void a_venue_must_have_a_name()
    {
        Scenario(() =>
        {
            Given(valid_inputs);
            And(a_null_venue_name);
            When(Validating(creating_a_venue));
            Then(Informs("VenueName cannot be null or empty"));
        });
        
        Scenario(() =>
        {
            Given(valid_inputs);
            And(an_empty_venue_name);
            When(Validating(creating_a_venue));
            Then(Informs("VenueName cannot be null or empty"));
        });
    }
    
    [Test]
    public void an_address_must_have_a_street()
    {
        Scenario(() =>
        {
            Given(valid_inputs);
            And(a_null_street);
            When(Validating(creating_a_venue));
            Then(Informs("Street is required"));
        });
        
        Scenario(() =>
        {
            Given(valid_inputs);
            And(an_empty_street);
            When(Validating(creating_a_venue));
            Then(Informs("Street is required"));
        });
    }
    
    [Test]
    public void an_address_must_have_a_city()
    {
        Scenario(() =>
        {
            Given(valid_inputs);
            And(a_null_city);
            When(Validating(creating_a_venue));
            Then(Informs("City is required"));
        });
        
        Scenario(() =>
        {
            Given(valid_inputs);
            And(an_empty_city);
            When(Validating(creating_a_venue));
            Then(Informs("City is required"));
        });
    }
    
    [Test]
    public void an_address_must_have_a_valid_uk_postcode()
    {
        Scenario(() =>
        {
            Given(valid_inputs);
            And(an_invalid_postcode);
            When(Validating(creating_a_venue));
            Then(Informs("Invalid UK postcode format"));
        });
    }
    
    [Test]
    public void capacity_must_be_at_least_1()
    {
        Given(valid_inputs);
        And(zero_capacity);
        When(Validating(creating_a_venue));
        Then(Informs("Capacity must be at least 1"));
    }
    
    [Test]
    public void capacity_cannot_exceed_50_seats()
    {
        Given(valid_inputs);
        And(capacity_of_51);
        When(Validating(creating_a_venue));
        Then(Informs("Capacity cannot exceed 50 seats"));
    }
}
