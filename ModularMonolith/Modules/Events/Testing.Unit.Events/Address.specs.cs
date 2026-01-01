using NUnit.Framework;

namespace Unit;

public partial class AddressSpecs
{
    [Test]
    public void can_create_valid_address()
    {
        Given(valid_address_inputs);
        When(creating_an_address);
        Then(the_address_is_created);
    }

    [Test]
    public void an_address_must_have_a_street()
    {
        Scenario(() =>
        {
            Given(valid_address_inputs);
            And(a_null_street);
            When(Validating(creating_an_address));
            Then(Informs("Street is required"));
        });

        Scenario(() =>
        {
            Given(valid_address_inputs);
            And(an_empty_street);
            When(Validating(creating_an_address));
            Then(Informs("Street is required"));
        });
    }

    [Test]
    public void an_address_must_have_a_city()
    {
        Scenario(() =>
        {
            Given(valid_address_inputs);
            And(a_null_city);
            When(Validating(creating_an_address));
            Then(Informs("City is required"));
        });

        Scenario(() =>
        {
            Given(valid_address_inputs);
            And(an_empty_city);
            When(Validating(creating_an_address));
            Then(Informs("City is required"));
        });
    }

    [Test]
    public void an_address_must_have_a_valid_uk_postcode()
    {
        Scenario(() =>
        {
            Given(valid_address_inputs);
            And(an_invalid_postcode);
            When(Validating(creating_an_address));
            Then(Informs("Invalid UK postcode format"));
        });
    }

    [Test]
    public void addresses_are_equal_when_values_are_same()
    {
        Given(two_addresses_with_same_values);
        When(comparing_addresses_for_equality);
        Then(addresses_are_equal);
    }

    [Test]
    public void addresses_are_equal_when_values_differ_only_in_case()
    {
        Given(two_addresses_with_different_case);
        When(comparing_addresses_for_equality);
        Then(addresses_are_equal);
    }

    [Test]
    public void addresses_are_not_equal_when_streets_differ()
    {
        Given(two_addresses_with_different_streets);
        When(comparing_addresses_for_equality);
        Then(addresses_are_not_equal);
    }

    [Test]
    public void addresses_are_not_equal_when_cities_differ()
    {
        Given(two_addresses_with_different_cities);
        When(comparing_addresses_for_equality);
        Then(addresses_are_not_equal);
    }

    [Test]
    public void addresses_are_not_equal_when_postcodes_differ()
    {
        Given(two_addresses_with_different_postcodes);
        When(comparing_addresses_for_equality);
        Then(addresses_are_not_equal);
    }

    [Test]
    public void addresses_have_same_hash_code_when_values_are_same()
    {
        Given(two_addresses_with_same_values);
        When(comparing_address_hash_codes);
        Then(address_hash_codes_are_equal);
    }

    [Test]
    public void addresses_have_same_hash_code_when_values_differ_only_in_case()
    {
        Given(two_addresses_with_different_case);
        When(comparing_address_hash_codes);
        Then(address_hash_codes_are_equal);
    }

    [Test]
    public void addresses_have_different_hash_code_when_values_differ()
    {
        Given(two_addresses_with_different_streets);
        When(comparing_address_hash_codes);
        Then(address_hash_codes_are_not_equal);
    }

    [Test]
    public void equality_operator_works_for_same_addresses()
    {
        Given(two_addresses_with_same_values);
        When(using_address_equality_operator);
        Then(addresses_are_equal);
    }

    [Test]
    public void inequality_operator_works_for_different_addresses()
    {
        Given(two_addresses_with_different_streets);
        When(using_address_inequality_operator);
        Then(addresses_are_not_equal);
    }

    [Test]
    public void can_convert_address_to_string()
    {
        Given(valid_address_inputs);
        When(creating_an_address);
        And(converting_address_to_string);
        Then(string_contains_street_city_and_postcode);
    }
}