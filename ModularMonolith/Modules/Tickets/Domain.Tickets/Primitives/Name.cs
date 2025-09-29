using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Domain.Tickets.Primitives;

[JsonConverter(typeof(NameConverter))]
public readonly record struct Name
{
    private string value { get; }

    public Name(string name)
    {
        Validation.BasedOn(errors =>
        {
            if (string.IsNullOrEmpty(name))
            {
                errors.Add("Name cannot be empty");
            }
            else if (Regex.IsMatch(name,@"[^a-zA-Z\s]", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                errors.Add("Name can only have alphabetical characters");
            }
        });
        value = name;
    }
    
    public override string ToString()
    {
        return value;
    }

    public static implicit operator string(Name name) => name.value;
    
    public static implicit operator Name(string name) => new(name);
}

public class NameConverter : JsonConverter<Name>
{
    public override void Write(Utf8JsonWriter writer, Name value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override Name Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()!;
    }
}