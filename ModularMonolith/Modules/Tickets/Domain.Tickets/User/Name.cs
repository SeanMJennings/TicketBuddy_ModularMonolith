using System.Text.RegularExpressions;
using Domain.ValueObjects;

namespace Domain.Tickets.User;

public readonly struct Name : IEquatable<Name>
{
    private readonly StringValueObject<Name> _value;

    public Name(string name)
    {
        _value = new StringValueObject<Name>(name);
        Validation.BasedOn(errors =>
        {
            if (Regex.IsMatch(name, @"[^a-zA-Z\s]", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                errors.Add("Name can only have alphabetical characters");
            }
        });
    }

    public override string ToString() => _value.ToString();
    public override bool Equals(object? obj) => obj is Name other && _value.Equals(other._value);
    public bool Equals(Name other) => _value.Equals(other._value);
    public override int GetHashCode() => _value.GetHashCode();
    public static bool operator ==(Name left, Name right) => left._value == right._value;
    public static bool operator !=(Name left, Name right) => left._value != right._value;
    public static implicit operator string(Name name) => name._value;
    public static implicit operator Name(string name) => new(name);
}