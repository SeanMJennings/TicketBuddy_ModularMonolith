using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Domain.Users.Primitives;

[JsonConverter(typeof(FullNameConverter))]
public readonly record struct FullName
{
    private string value { get; }

    public FullName(string name)
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

    public static implicit operator string(FullName fullName) => fullName.value;
    
    public static implicit operator FullName(string name) => new(name);
}

public class FullNameConverter : JsonConverter<FullName>
{
    public override void Write(Utf8JsonWriter writer, FullName value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override FullName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()!;
    }
}