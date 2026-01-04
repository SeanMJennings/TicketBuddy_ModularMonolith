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

    [Test]
    public async Task cannot_create_venue_with_duplicate_address()
    {
              Given(valid_inputs);
              And(an_existing_venue_at_the_same_address);
        await When(Validating(validating_address_uniqueness));
              Then(Informs("A venue already exists at this address"));
    }

    [Test]
    public void can_update_venue_name()
    {
        Given(a_venue_exists);
        And(a_new_venue_name);
        When(updating_the_venue_name);
        Then(the_venue_name_is_updated);
    }

    [Test]
    public void can_update_venue_address()
    {
        Given(a_venue_exists);
        And(a_new_address);
        When(updating_the_venue_address);
        Then(the_venue_address_is_updated);
    }

    [Test]
    public void can_update_venue_capacity()
    {
        Given(a_venue_exists);
        And(a_new_capacity);
        When(updating_the_venue_capacity);
        Then(the_venue_capacity_is_updated);
    }

    [Test]
    public void cannot_update_capacity_to_zero()
    {
        Given(a_venue_exists);
        And(zero_capacity);
        When(Validating(updating_the_venue_capacity));
        Then(Informs("Capacity must be at least 1"));
    }

    [Test]
    public void cannot_update_capacity_above_50()
    {
        Given(a_venue_exists);
        And(capacity_of_51);
        When(Validating(updating_the_venue_capacity));
        Then(Informs("Capacity cannot exceed 50 seats"));
    }
}