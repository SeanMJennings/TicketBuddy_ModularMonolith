using BDD;
using NUnit.Framework;

namespace Domain;

public partial class EventNameSpecs : Specification
{
    [Test]
    public void can_create_valid_event_name()
    {
        Given(a_valid_event_name);
        When(creating_an_event_name);
        Then(the_event_name_is_created);
    }

    [Test]
    public void event_names_with_special_characters_are_invalid()
    {
        Given(an_event_name_with_special_characters);
        When(Validating(creating_an_event_name));
        Then(Informs("Name can only have alphanumerical characters"));
    }

    [Test]
    public void event_names_are_equal_when_values_are_same()
    {
        Given(two_event_names_with_same_value);
        When(comparing_event_names_for_equality);
        Then(event_names_are_equal);
    }

    [Test]
    public void event_names_are_equal_when_values_differ_only_in_case()
    {
        Given(two_event_names_with_different_case);
        When(comparing_event_names_for_equality);
        Then(event_names_are_equal);
    }

    [Test]
    public void event_names_are_not_equal_when_values_differ()
    {
        Given(two_event_names_with_different_values);
        When(comparing_event_names_for_equality);
        Then(event_names_are_not_equal);
    }

    [Test]
    public void event_names_have_same_hash_code_when_values_are_same()
    {
        Given(two_event_names_with_same_value);
        When(comparing_event_name_hash_codes);
        Then(event_name_hash_codes_are_equal);
    }

    [Test]
    public void event_names_have_same_hash_code_when_values_differ_only_in_case()
    {
        Given(two_event_names_with_different_case);
        When(comparing_event_name_hash_codes);
        Then(event_name_hash_codes_are_equal);
    }

    [Test]
    public void event_names_can_be_used_as_dictionary_keys()
    {
        Given(a_dictionary_with_event_name_keys);
        When(adding_event_names_with_same_value_different_case);
        Then(dictionary_contains_one_entry);
    }

    [Test]
    public void an_event_name_is_not_equal_to_a_non_event_name_object()
    {
        Given(a_valid_event_name);
        And(creating_an_event_name);
        When(comparing_event_name_to_non_event_name_object);
        Then(event_names_are_not_equal);
    }
}