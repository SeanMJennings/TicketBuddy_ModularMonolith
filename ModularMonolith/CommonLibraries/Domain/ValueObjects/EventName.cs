using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

[JsonConverter(typeof(EventNameConverter))]
public readonly struct EventName : IEquatable<EventName>
{
    private readonly StringValueObject<EventName> _value;

    public EventName(string name)
    {
        _value = new StringValueObject<EventName>(name);
        Validation.BasedOn(errors =>
        {
            if (Regex.IsMatch(name,@"[^a-zA-Z0-9\s]", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                errors.Add("Name can only have alphanumerical characters");
            }
        });
    }
    
    public override string ToString() => _value.ToString();

    public override bool Equals(object? obj) => obj is EventName other && _value.Equals(other._value);

    public bool Equals(EventName other) => _value.Equals(other._value);

    public override int GetHashCode() => _value.GetHashCode();

    public static bool operator ==(EventName left, EventName right) => left._value == right._value;

    public static bool operator !=(EventName left, EventName right) => left._value != right._value;

    public static implicit operator string(EventName eventName) => eventName._value;

    public static implicit operator EventName(string name) => new(name);
}

public class EventNameConverter : JsonConverter<EventName>
{
    public override void Write(Utf8JsonWriter writer, EventName value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override EventName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()!;
    }
}