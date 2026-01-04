using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Events.Venue;

public class AddressConverter : JsonConverter<Address>
{
    public override Address Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        string? street = null;
        string? city = null;
        string? postcode = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token");

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

        if (street == null || city == null || postcode == null)
            throw new JsonException("Missing required Address properties");

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
