using System.Text.Json.Serialization;
using Domain.ValueObjects;

namespace Domain.Events.Venue;

[JsonConverter(typeof(VenueNameConverter))]
public readonly struct VenueName : IEquatable<VenueName>
{
    private readonly StringValueObject<VenueName> _value;

    public VenueName(string name)
    {
        _value = new StringValueObject<VenueName>(name);
    }

    public override string ToString() => _value.ToString();
    public override bool Equals(object? obj) => obj is VenueName other && _value.Equals(other._value);
    public bool Equals(VenueName other) => _value.Equals(other._value);
    public override int GetHashCode() => _value.GetHashCode();
    public static bool operator ==(VenueName left, VenueName right) => left._value == right._value;
    public static bool operator !=(VenueName left, VenueName right) => left._value != right._value;
    public static implicit operator string(VenueName venueName) => venueName._value;
    public static implicit operator VenueName(string name) => new(name);
}

public class VenueNameConverter : StringValueObjectJsonConverter<VenueName>
{
    protected override VenueName CreateFromString(string value)
    {
        return new VenueName(value);
    }
}
