using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Domain.Tickets.Primitives;

[JsonConverter(typeof(EventNameConverter))]
public readonly record struct EventName
{
    private string value { get; }

    public EventName(string name)
    {
        Validation.BasedOn(errors =>
        {
            if (string.IsNullOrEmpty(name))
            {
                errors.Add("Name cannot be empty");
            }
            else if (Regex.IsMatch(name,@"[^a-zA-Z0-9\s]", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                errors.Add("Name can only have alphanumerical characters");
            }
        });
        value = name;
    }
    
    public override string ToString()
    {
        return value;
    }

    public static implicit operator string(EventName eventName) => eventName.value;
    
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