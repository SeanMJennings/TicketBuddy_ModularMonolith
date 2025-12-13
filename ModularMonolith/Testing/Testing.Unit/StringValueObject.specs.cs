using BDD;
using NUnit.Framework;

namespace Domain;

public partial class StringValueObjectSpecs : Specification
{
    [Test]
    public void string_value_objects_are_equal_when_values_are_same()
    {
        Given(two_value_objects_with_same_value);
        When(comparing_for_equality);
        Then(they_are_equal);
    }
    
    [Test]
    public void string_value_objects_are_equal_when_values_differ_only_in_case()
    {
        Given(two_value_objects_with_different_case);
        When(comparing_for_equality);
        Then(they_are_equal);
    }
    
    [Test]
    public void string_value_objects_are_not_equal_when_values_differ()
    {
        Given(two_value_objects_with_different_values);
        When(comparing_for_equality);
        Then(they_are_not_equal);
    }
    
    [Test]
    public void string_value_objects_have_same_hash_code_when_values_are_same()
    {
        Given(two_value_objects_with_same_value);
        When(comparing_hash_codes);
        Then(hash_codes_are_equal);
    }
    
    [Test]
    public void string_value_objects_have_same_hash_code_when_values_differ_only_in_case()
    {
        Given(two_value_objects_with_different_case);
        When(comparing_hash_codes);
        Then(hash_codes_are_equal);
    }
    
    [Test]
    public void string_value_objects_have_different_hash_code_when_values_differ()
    {
        Given(two_value_objects_with_different_values);
        When(comparing_hash_codes);
        Then(hash_codes_are_not_equal);
    }
    
    [Test]
    public void can_convert_value_object_to_string()
    {
        Given(a_value_object_with_value);
        When(converting_to_string);
        Then(string_matches_original_value);
    }
    
    [Test]
    public void equality_operator_works_for_same_values()
    {
        Given(two_value_objects_with_same_value);
        When(using_equality_operator);
        Then(they_are_equal);
    }
    
    [Test]
    public void equality_operator_works_for_different_case()
    {
        Given(two_value_objects_with_different_case);
        When(using_equality_operator);
        Then(they_are_equal);
    }
    
    [Test]
    public void inequality_operator_works_for_different_values()
    {
        Given(two_value_objects_with_different_values);
        When(using_inequality_operator);
        Then(they_are_not_equal);
    }
    
    [Test]
    public void cannot_create_value_object_with_null_value()
    {
        Given(a_null_value);
        When(Validating(creating_value_object));
        Then(Informs("Value cannot be null or empty"));
    }
    
    [Test]
    public void cannot_create_value_object_with_empty_value()
    {
        Given(an_empty_value);
        When(Validating(creating_value_object));
        Then(Informs("Value cannot be null or empty"));
    }
    
    [Test]
    public void can_use_in_dictionary_as_key()
    {
        Given(a_dictionary_with_value_object_keys);
        When(adding_items_with_case_insensitive_keys);
        Then(dictionary_contains_one_item);
    }
    
    [Test]
    public void can_use_in_hashset()
    {
        Given(a_hashset_of_value_objects);
        When(adding_items_with_case_insensitive_values);
        Then(hashset_contains_one_item);
    }
}