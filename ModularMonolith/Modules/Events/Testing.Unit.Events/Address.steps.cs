using System.Text.Json;
using BDD;
using Domain.Events.Venue;
using Shouldly;

namespace Unit;

public partial class AddressSpecs : Specification
{
    private string street = null!;
    private string city = null!;
    private string postcode = null!;
    private Address address;
    private Address address1;
    private Address address2;
    private bool equalityResult;
    private bool inequalityResult;
    private int hashCode1;
    private int hashCode2;
    private string? stringResult;
    private string? jsonInput;
    private string? serializedJson;
    private Address deserializedAddress;

    private const string valid_street = "Peninsula Square";
    private const string valid_city = "London";
    private const string valid_postcode = "SE10 0DX";
    private const string valid_street_lowercase = "peninsula square";
    private const string valid_city_lowercase = "london";
    private const string valid_postcode_lowercase = "se10 0dx";
    private const string different_street = "123 Main Street";
    private const string different_city = "Manchester";
    private const string different_postcode = "M1 1AA";

    protected override void before_each()
    {
        street = null!;
        city = null!;
        postcode = null!;
        address = default;
        address1 = default;
        address2 = default;
        equalityResult = false;
        inequalityResult = false;
        hashCode1 = 0;
        hashCode2 = 0;
        stringResult = null;
        jsonInput = null;
        serializedJson = null;
        deserializedAddress = default;
    }

    private void valid_address_inputs()
    {
        street = valid_street;
        city = valid_city;
        postcode = valid_postcode;
    }

    private void a_null_street()
    {
        street = null!;
    }

    private void an_empty_street()
    {
        street = string.Empty;
    }

    private void a_null_city()
    {
        city = null!;
    }

    private void an_empty_city()
    {
        city = string.Empty;
    }

    private void an_invalid_postcode()
    {
        postcode = "INVALID";
    }

    private void creating_an_address()
    {
        address = new Address(street, city, postcode);
    }

    private void the_address_is_created()
    {
        address.Street.ShouldBe(valid_street);
        address.City.ShouldBe(valid_city);
        address.Postcode.ShouldBe(valid_postcode.ToUpperInvariant());
    }

    private void two_addresses_with_same_values()
    {
        address1 = new Address(valid_street, valid_city, valid_postcode);
        address2 = new Address(valid_street, valid_city, valid_postcode);
    }

    private void two_addresses_with_different_case()
    {
        address1 = new Address(valid_street, valid_city, valid_postcode);
        address2 = new Address(valid_street_lowercase, valid_city_lowercase, valid_postcode_lowercase);
    }

    private void two_addresses_with_different_streets()
    {
        address1 = new Address(valid_street, valid_city, valid_postcode);
        address2 = new Address(different_street, valid_city, valid_postcode);
    }

    private void two_addresses_with_different_cities()
    {
        address1 = new Address(valid_street, valid_city, valid_postcode);
        address2 = new Address(valid_street, different_city, valid_postcode);
    }

    private void two_addresses_with_different_postcodes()
    {
        address1 = new Address(valid_street, valid_city, valid_postcode);
        address2 = new Address(valid_street, valid_city, different_postcode);
    }

    private void comparing_addresses_for_equality()
    {
        equalityResult = address1.Equals(address2);
    }

    private void comparing_address_hash_codes()
    {
        hashCode1 = address1.GetHashCode();
        hashCode2 = address2.GetHashCode();
    }

    private void using_address_equality_operator()
    {
        equalityResult = address1 == address2;
    }

    private void using_address_inequality_operator()
    {
        inequalityResult = address1 != address2;
    }

    private void converting_address_to_string()
    {
        stringResult = address.ToString();
    }

    private void addresses_are_equal()
    {
        equalityResult.ShouldBeTrue();
    }

    private void addresses_are_not_equal()
    {
        if (inequalityResult)
        {
            inequalityResult.ShouldBeTrue();
        }
        else
        {
            equalityResult.ShouldBeFalse();
        }
    }

    private void address_hash_codes_are_equal()
    {
        hashCode1.ShouldBe(hashCode2);
    }

    private void address_hash_codes_are_not_equal()
    {
        hashCode1.ShouldNotBe(hashCode2);
    }

    private void string_contains_street_city_and_postcode()
    {
        stringResult!.ShouldContain(valid_street);
        stringResult!.ShouldContain(valid_city);
        stringResult!.ShouldContain(valid_postcode.ToUpperInvariant());
    }

    private void a_null_postcode() => postcode = null!;
    private void an_empty_postcode() => postcode = string.Empty;

    private void comparing_address_to_non_address_object()
    {
        object nonAddress = "not an address";
        equalityResult = address.Equals(nonAddress);
    }

    private void valid_address_json() =>
        jsonInput = $@"{{""Street"":""{valid_street}"",""City"":""{valid_city}"",""Postcode"":""{valid_postcode}""}}";

    private void valid_address_json_with_camel_case() =>
        jsonInput = $@"{{""street"":""{valid_street}"",""city"":""{valid_city}"",""postcode"":""{valid_postcode}""}}";

    private void address_json_with_null_street() =>
        jsonInput = $@"{{""Street"":null,""City"":""{valid_city}"",""Postcode"":""{valid_postcode}""}}";

    private void serializing_address_to_json() =>
        serializedJson = JsonSerializer.Serialize(address);

    private void deserializing_json_to_address() =>
        deserializedAddress = JsonSerializer.Deserialize<Address>(jsonInput!);

    private void json_contains_all_address_properties()
    {
        serializedJson!.ShouldContain($@"""Street"":""{valid_street}""");
        serializedJson!.ShouldContain($@"""City"":""{valid_city}""");
        serializedJson!.ShouldContain($@"""Postcode"":""{valid_postcode.ToUpperInvariant()}""");
    }

    private void deserialized_address_has_correct_values()
    {
        deserializedAddress.Street.ShouldBe(valid_street);
        deserializedAddress.City.ShouldBe(valid_city);
        deserializedAddress.Postcode.ShouldBe(valid_postcode.ToUpperInvariant());
    }
}