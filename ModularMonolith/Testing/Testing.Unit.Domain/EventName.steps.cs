using Domain.ValueObjects;
using Shouldly;

namespace Domain;

public partial class EventNameSpecs
{
    private EventName eventName1;
    private EventName eventName2;
    private string? eventNameInput;
    private bool equalityResult;
    private int hashCode1;
    private int hashCode2;
    private Dictionary<EventName, string>? dictionary;

    private const string SampleEventName = "Summer Concert 2025";
    private const string SampleEventNameUpper = "SUMMER CONCERT 2025";
    private const string SampleEventNameLower = "summer concert 2025";
    private const string DifferentEventName = "Winter Gala 2025";
    private const string EventNameWithSpecialChars = "Summer Concert!";

    private void a_valid_event_name() => eventNameInput = SampleEventName;
    private void an_event_name_with_special_characters() => eventNameInput = EventNameWithSpecialChars;

    private void creating_an_event_name() => eventName1 = new EventName(eventNameInput!);

    private void the_event_name_is_created() => eventName1.ToString().ShouldBe(SampleEventName);

    private void two_event_names_with_same_value()
    {
        eventName1 = new EventName(SampleEventName);
        eventName2 = new EventName(SampleEventName);
    }

    private void two_event_names_with_different_case()
    {
        eventName1 = new EventName(SampleEventNameLower);
        eventName2 = new EventName(SampleEventNameUpper);
    }

    private void two_event_names_with_different_values()
    {
        eventName1 = new EventName(SampleEventName);
        eventName2 = new EventName(DifferentEventName);
    }

    private void comparing_event_names_for_equality() => equalityResult = eventName1.Equals(eventName2);

    private void comparing_event_name_hash_codes()
    {
        hashCode1 = eventName1.GetHashCode();
        hashCode2 = eventName2.GetHashCode();
    }

    private void a_dictionary_with_event_name_keys() => dictionary = new Dictionary<EventName, string>();

    private void adding_event_names_with_same_value_different_case()
    {
        dictionary![new EventName(SampleEventNameLower)] = "first";
        dictionary[new EventName(SampleEventNameUpper)] = "second";
    }

    private void comparing_event_name_to_non_event_name_object()
    {
        object nonEventName = "not an event name";
        equalityResult = eventName1.Equals(nonEventName);
    }

    private void event_names_are_equal() => equalityResult.ShouldBeTrue();

    private void event_names_are_not_equal() => equalityResult.ShouldBeFalse();

    private void event_name_hash_codes_are_equal() => hashCode1.ShouldBe(hashCode2);

    private void dictionary_contains_one_entry()
    {
        dictionary!.Count.ShouldBe(1);
        dictionary[new EventName(SampleEventName)].ShouldBe("second");
    }
}