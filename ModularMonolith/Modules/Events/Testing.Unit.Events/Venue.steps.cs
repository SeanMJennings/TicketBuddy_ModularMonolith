using BDD;
using Domain.Events.Venue;
using Domain.Exceptions;
using Shouldly;

namespace Unit;

public partial class VenueSpecs : Specification
{
    private Guid id;
    private string venueName = null!;
    private string street = null!;
    private string city = null!;
    private string postcode = null!;
    private uint capacity;
    private Guid? excludeVenueId;
    private Venue? otherVenue;
    private Venue venue = null!;
    private IEnumerable<Venue> venues = [];

    private const string valid_venue_name = "The O2 Arena";
    private const string valid_street = "Peninsula Square";
    private const string valid_city = "London";
    private const string valid_postcode = "SE10 0DX";
    private const uint valid_capacity = 20;
    private const string updated_venue_name = "Updated Arena";
    private const string updated_street = "New Street";
    private const string updated_city = "Manchester";
    private const string updated_postcode = "M1 1AA";
    private const uint updated_capacity = 35;

    protected override void before_each()
    {
        base.before_each();
        id = Guid.CreateVersion7();
        venueName = null!;
        street = null!;
        city = null!;
        postcode = null!;
        capacity = 0;
        venue = null!;
        venues = [];
        excludeVenueId = null;
        otherVenue = null;
    }

    private void valid_inputs()
    {
        venueName = valid_venue_name;
        street = valid_street;
        city = valid_city;
        postcode = valid_postcode;
        capacity = valid_capacity;
    }

    private void a_null_venue_name()
    {
        venueName = null!;
    }

    private void an_empty_venue_name()
    {
        venueName = string.Empty;
    }


    private void zero_capacity()
    {
        capacity = 0;
    }

    private void capacity_of_51()
    {
        capacity = 51;
    }

    private void creating_a_venue()
    {
        var address = new Address(street, city, postcode);
        venue = new Venue(id, new VenueName(venueName), address, capacity);
    }

    private void the_venue_is_created()
    {
        venue.Id.ShouldBe(id);
        venue.Name.ToString().ShouldBe(valid_venue_name);
        (venue.Name == valid_venue_name).ShouldBeTrue();
        (venue.Name != valid_venue_name).ShouldBeFalse();
        venue.Name.GetHashCode().ShouldBe(new VenueName(venueName).GetHashCode());
        venue.Address.Street.ShouldBe(valid_street);
        venue.Address.City.ShouldBe(valid_city);
        venue.Address.Postcode.ShouldBe(valid_postcode.ToUpperInvariant());
        venue.Capacity.ShouldBe(valid_capacity);
    }

    private void an_existing_venue_at_the_same_address()
    {
        venues =
        [
            new Venue(
                Guid.CreateVersion7(),
                new VenueName("Different Venue Name"),
                new Address(valid_street, valid_city, valid_postcode),
                30
            )
        ];
    }

    private void validating_address_uniqueness()
    {
        var address = new Address(street, city, postcode);
        VenuesValidator.CheckAddressUniqueness(address, venues, excludeVenueId);
    }

    private void a_venue_exists()
    {
        valid_inputs();
        creating_a_venue();
    }

    private void a_new_venue_name()
    {
        venueName = updated_venue_name;
    }

    private void a_new_address()
    {
        street = updated_street;
        city = updated_city;
        postcode = updated_postcode;
    }

    private void a_new_capacity()
    {
        capacity = updated_capacity;
    }

    private void updating_the_venue_name()
    {
        venue.UpdateName(new VenueName(venueName));
    }

    private void updating_the_venue_address()
    {
        var address = new Address(street, city, postcode);
        venue.UpdateAddress(address);
    }

    private void updating_the_venue_capacity()
    {
        venue.UpdateCapacity(capacity);
    }

    private void the_venue_name_is_updated()
    {
        venue.Name.ToString().ShouldBe(updated_venue_name);
    }

    private void the_venue_address_is_updated()
    {
        venue.Address.Street.ShouldBe(updated_street);
        venue.Address.City.ShouldBe(updated_city);
        venue.Address.Postcode.ShouldBe(updated_postcode.ToUpperInvariant());
    }

    private void the_venue_capacity_is_updated()
    {
        venue.Capacity.ShouldBe(updated_capacity);
    }

    private Venue? retrievedVenue;

    private void checking_venue_exists()
    {
        retrievedVenue = VenuesValidator.CheckVenueExists(venue, id);
    }

    private void the_venue_is_returned()
    {
        retrievedVenue.ShouldNotBeNull();
        retrievedVenue.Id.ShouldBe(id);
    }

    private void a_venue_that_does_not_exist()
    {
        venue = null!;
        id = Guid.CreateVersion7();
    }

    private void no_existing_venues()
    {
        venues = [];
    }

    private void the_venue_is_in_the_repository()
    {
        venues = [venue];
    }

    private void excluding_the_current_venue_from_uniqueness_check()
    {
        excludeVenueId = id;
    }

    private void another_venue_at_different_address()
    {
        otherVenue = new Venue(
            Guid.CreateVersion7(),
            new VenueName("Different Venue"),
            new Address("Different Street", "Different City", "M1 1AA"),
            30
        );
        venues = [venue, otherVenue];
    }

    private void updating_to_the_other_venues_address()
    {
        street = otherVenue!.Address.Street;
        city = otherVenue.Address.City;
        postcode = otherVenue.Address.Postcode;
    }

    private static void an_entity_not_found_error_is_thrown()
    {
        error.ShouldBeOfType<EntityNotFoundException>();
    }
}