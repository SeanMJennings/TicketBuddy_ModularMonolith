using BDD;
using Domain.Events.Venue;
using Moq;
using Shouldly;

namespace Unit;

public partial class VenueSpecs : AsyncSpecification
{
    private Guid id;
    private string venueName = null!;
    private string street = null!;
    private string city = null!;
    private string postcode = null!;
    private uint capacity;
    private Venue venue = null!;
    private Mock<IPersistVenues> venueRepository = null!;
    private VenuesValidator validator = null!;

    private const string valid_venue_name = "The O2 Arena";
    private const string valid_street = "Peninsula Square";
    private const string valid_city = "London";
    private const string valid_postcode = "SE10 0DX";
    private const uint valid_capacity = 20;

    protected override Task before_each()
    {
        id = Guid.NewGuid();
        venueName = null!;
        street = null!;
        city = null!;
        postcode = null!;
        capacity = 0;
        venue = null!;
        venueRepository = new Mock<IPersistVenues>();
        validator = new VenuesValidator(venueRepository.Object);
        return Task.CompletedTask;
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
        venue.Address.Street.ShouldBe(valid_street);
        venue.Address.City.ShouldBe(valid_city);
        venue.Address.Postcode.ShouldBe(valid_postcode.ToUpperInvariant());
        venue.Capacity.ShouldBe(valid_capacity);
    }

    private void an_existing_venue_at_the_same_address()
    {
        var existingVenue = new Venue(
            Guid.NewGuid(),
            new VenueName("Different Venue Name"),
            new Address(valid_street, valid_city, valid_postcode),
            30
        );
        venueRepository.Setup(x => x.GetAll()).ReturnsAsync([existingVenue]);
    }

    private async Task validating_address_uniqueness()
    {
        var address = new Address(street, city, postcode);
        await validator.CheckAddressUniqueness(address);
    }
}