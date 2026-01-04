using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Domain.Events.Venue;

[JsonConverter(typeof(AddressConverter))]
public readonly struct Address : IEquatable<Address>
{
    public Address(string street, string city, string postcode)
    {
        Validation.BasedOn(errors =>
        {
            if (string.IsNullOrWhiteSpace(street)) errors.Add("Street is required");
            if (string.IsNullOrWhiteSpace(city)) errors.Add("City is required");
            if (!IsValidUkPostcode(postcode)) errors.Add("Invalid UK postcode format");
        });

        Street = street;
        City = city;
        Postcode = postcode.ToUpperInvariant();
    }

    public string Street { get; }

    public string City { get; }

    public string Postcode { get; }

    private static bool IsValidUkPostcode(string postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode)) return false;

        const string pattern = @"^[A-Z]{1,2}[0-9]{1,2}[A-Z]?\s?[0-9][A-Z]{2}$";
        return Regex.IsMatch(
            postcode.ToUpperInvariant(),
            pattern,
            RegexOptions.None,
            TimeSpan.FromMilliseconds(100));
    }

    public bool Equals(Address other) =>
        string.Equals(Street, other.Street, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(City, other.City, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(Postcode, other.Postcode, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj is Address other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(
        Street?.ToUpperInvariant(),
        City?.ToUpperInvariant(),
        Postcode?.ToUpperInvariant());

    public static bool operator ==(Address left, Address right) => left.Equals(right);
    public static bool operator !=(Address left, Address right) => !left.Equals(right);

    public override string ToString() => $"{Street}, {City}, {Postcode}";
}

public class AddressConverter : JsonConverter<Address>
{
    public override Address Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected StartObject token");

        string? street = null;
        string? city = null;
        string? postcode = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) break;

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected PropertyName token");

            var propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(Address.Street):
                case "street":
                    street = reader.GetString();
                    break;
                case nameof(Address.City):
                case "city":
                    city = reader.GetString();
                    break;
                case nameof(Address.Postcode):
                case "postcode":
                    postcode = reader.GetString();
                    break;
            }
        }

        if (street == null || city == null || postcode == null) throw new JsonException("Missing required Address properties");

        return new Address(street, city, postcode);
    }

    public override void Write(Utf8JsonWriter writer, Address value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(nameof(Address.Street), value.Street);
        writer.WriteString(nameof(Address.City), value.City);
        writer.WriteString(nameof(Address.Postcode), value.Postcode);
        writer.WriteEndObject();
    }
}