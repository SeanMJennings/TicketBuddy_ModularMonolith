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
    public void cannot_create_venue_with_duplicate_address()
    {
        Given(valid_inputs);
        And(an_existing_venue_at_the_same_address);
        When(Validating(validating_address_uniqueness));
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

    [Test]
    public void can_check_venue_exists()
    {
        Given(a_venue_exists);
        When(checking_venue_exists);
        Then(the_venue_is_returned);
    }

    [Test]
    public void check_venue_exists_throws_when_venue_not_found()
    {
        Given(a_venue_that_does_not_exist);
        When(Validating(checking_venue_exists));
        Then(Informs($"Venue with id {id} was not found."));
        And(an_entity_not_found_error_is_thrown);
    }

    [Test]
    public void can_create_venue_with_unique_address()
    {
        Given(valid_inputs);
        And(no_existing_venues);
        When(validating_address_uniqueness);
    }

    [Test]
    public void can_update_venue_keeping_same_address()
    {
        Given(a_venue_exists);
        And(the_venue_is_in_the_repository);
        And(excluding_the_current_venue_from_uniqueness_check);
        When(validating_address_uniqueness);
    }

    [Test]
    public void cannot_update_venue_to_another_venues_address()
    {
        Given(a_venue_exists);
        And(another_venue_at_different_address);
        And(excluding_the_current_venue_from_uniqueness_check);
        And(updating_to_the_other_venues_address);
        When(Validating(validating_address_uniqueness));
        Then(Informs("A venue already exists at this address"));
    }
}