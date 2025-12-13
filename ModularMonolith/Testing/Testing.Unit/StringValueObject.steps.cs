using Domain.ValueObjects;
using Shouldly;

namespace Domain;

public readonly struct TestValue(string value) : IEquatable<TestValue>
{
    private readonly StringValueObject<TestValue> _value = new(value);

    public override string ToString() => _value.ToString();
    
    public override bool Equals(object? obj) => obj is TestValue other && _value.Equals(other._value);
    
    public bool Equals(TestValue other) => _value.Equals(other._value);
    
    public override int GetHashCode() => _value.GetHashCode();
    
    public static bool operator ==(TestValue left, TestValue right) => left._value == right._value;
    
    public static bool operator !=(TestValue left, TestValue right) => left._value != right._value;
}

public partial class StringValueObjectSpecs
{
    private TestValue value1;
    private TestValue value2;
    private bool equalityResult;
    private bool inequalityResult;
    private int hashCode1;
    private int hashCode2;
    private string? stringResult;
    private Dictionary<TestValue, string>? dictionary;
    private HashSet<TestValue>? hashset;
    private string? testValueInput;
    
    private const string SampleValue = "Test Value";
    private const string SampleValueUpperCase = "TEST VALUE";
    private const string SampleValueLowerCase = "test value";
    private const string DifferentValue = "Different Value";

    private void two_value_objects_with_same_value()
    {
        value1 = new TestValue(SampleValue);
        value2 = new TestValue(SampleValue);
    }
    
    private void two_value_objects_with_different_case()
    {
        value1 = new TestValue(SampleValueLowerCase);
        value2 = new TestValue(SampleValueUpperCase);
    }
    
    private void two_value_objects_with_different_values()
    {
        value1 = new TestValue(SampleValue);
        value2 = new TestValue(DifferentValue);
    }
    
    private void a_value_object_with_value()
    {
        value1 = new TestValue(SampleValue);
    }
    
    private void a_null_value()
    {
        testValueInput = null;
    }
    
    private void an_empty_value()
    {
        testValueInput = string.Empty;
    }
    
    private void creating_value_object()
    {
        value1 = new TestValue(testValueInput!);
    }
    
    private void a_dictionary_with_value_object_keys()
    {
        dictionary = new Dictionary<TestValue, string>();
    }
    
    private void a_hashset_of_value_objects()
    {
        hashset = new HashSet<TestValue>();
    }
    
    private void comparing_for_equality()
    {
        equalityResult = value1.Equals(value2);
    }
    
    private void comparing_hash_codes()
    {
        hashCode1 = value1.GetHashCode();
        hashCode2 = value2.GetHashCode();
    }
    
    private void converting_to_string()
    {
        stringResult = value1.ToString();
    }
    
    private void using_equality_operator()
    {
        equalityResult = value1 == value2;
    }
    
    private void using_inequality_operator()
    {
        inequalityResult = value1 != value2;
    }
    
    private void adding_items_with_case_insensitive_keys()
    {
        dictionary![new TestValue(SampleValueLowerCase)] = "first";
        dictionary[new TestValue(SampleValueUpperCase)] = "second";
    }
    
    private void adding_items_with_case_insensitive_values()
    {
        hashset!.Add(new TestValue(SampleValueLowerCase));
        hashset.Add(new TestValue(SampleValueUpperCase));
        hashset.Add(new TestValue(SampleValue));
    }
    
    private void they_are_equal()
    {
        equalityResult.ShouldBeTrue();
    }
    
    private void they_are_not_equal()
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
    
    private void hash_codes_are_equal()
    {
        hashCode1.ShouldBe(hashCode2);
    }
    
    private void hash_codes_are_not_equal()
    {
        hashCode1.ShouldNotBe(hashCode2);
    }
    
    private void string_matches_original_value()
    {
        stringResult.ShouldBe(SampleValue);
    }
    
    private void dictionary_contains_one_item()
    {
        dictionary!.Count.ShouldBe(1);
        dictionary[new TestValue(SampleValue)].ShouldBe("second");
    }
    
    private void hashset_contains_one_item()
    {
        hashset!.Count.ShouldBe(1);
    }
}