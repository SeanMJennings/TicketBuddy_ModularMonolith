using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Domain.ValueObjects;

namespace Domain.Tickets.User;

[JsonConverter(typeof(EmailConverter))]
public readonly struct Email : IEquatable<Email>
{
    private readonly StringValueObject<Email> _value;

    public Email(string email)
    {
        _value = new StringValueObject<Email>(email);
        Validation.BasedOn(errors =>
        {
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                errors.Add("Email must be valid");
            }
        });
    }

    public override string ToString() => _value.ToString();
    public override bool Equals(object? obj) => obj is Email other && _value.Equals(other._value);
    public bool Equals(Email other) => _value.Equals(other._value);
    public override int GetHashCode() => _value.GetHashCode();
    public static bool operator ==(Email left, Email right) => left._value == right._value;
    public static bool operator !=(Email left, Email right) => left._value != right._value;
    public static implicit operator string(Email email) => email._value;
    public static implicit operator Email(string email) => new(email);
}

public class EmailConverter : StringValueObjectJsonConverter<Email>
{
    protected override Email CreateFromString(string value)
    {
        return new Email(value);
    }
}